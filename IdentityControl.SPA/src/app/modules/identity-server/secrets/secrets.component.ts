import { Component, OnInit, ViewChild } from '@angular/core';
import { Secret, SecretType } from '../../../models/identity-server/secret';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatSelect, MatSelectChange } from '@angular/material/select';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import 'src/app/helpers/dateExtensions';
import { DateTime } from 'luxon';
import { getDateFromLocale } from '../../../helpers/dateExtensions';
import { charSet64, Entropy } from 'entropy-string';
import { IdentityServerBaseService } from '../../../services/identity-server/identity-server-base-service';
import { BaseOption } from '../../../models/option';
import { IdentityServerSecretService } from '../../../services/identity-server/identity-server-secret.service';
import { SortDirection } from '../../../models/sortDirection';
import { SearchService } from '../../../services/search.service';
import { ActivatedRoute, Router } from '@angular/router';

enum SecretFilter {
  Active,
  Expired
}

@Component({
  selector: 'app-secrets',
  templateUrl: './secrets.component.html',
  styleUrls: ['../identity-server.scss', './secrets.component.scss']
})
export class SecretsComponent
  extends IdentityServerBaseService<Secret, IdentityServerSecretService>
  implements OnInit {
  /// FOR BASE
  displayedColumns: (keyof Secret)[] = [
    'selected',
    'value',
    'description',
    'clientName',
    'apiResourceName',
    'type',
    'created',
    'expiration'
  ];
  /// FOR CURRENT PAGE
  createdAt: any;
  secretType: string;
  customDate: boolean;
  expireSelectValue: string;
  starsPlaceholder = '*********************************************';
  pageTitle = 'Secrets';
  clientOptions: BaseOption<number>[];
  apiResourceOptions: BaseOption<number>[];
  @ViewChild('filterOption') filterOptionRef: MatSelect;

  constructor(
    public httpService: IdentityServerSecretService,
    public formBuilder: FormBuilder,
    public searchService: SearchService,
    public route: ActivatedRoute,
    public router: Router
  ) {
    super();
    this.queryParams.sortColumn = 'Created';
    this.queryParams.sortDirection = SortDirection.Dsc;
    this.httpService.endpoint = 'client-secret';
  }

  // These are also present in the base, they are here to offer type support in the template
  get itemSelected() {
    return this.selectedRows[0];
  }

  set itemSelected(value) {
    this.selectedRows[0] = value;
  }

  ngOnInit(): void {
    this.currentSection = this.route.snapshot.queryParams.relation ?? 'client';
    this.setSecretType(this.currentSection);
    this.getFormDropdownOptions();
    this.initialize();
  }

  buildForm(item: Secret = null) {
    this.isFormVisible = true;
    this.setExpireOption(item);

    if (!item) {
      this.addMode = true;
      this.createdAt = DateTime.utc();
      this.editForm = this.setUpAddForm();
    } else {
      this.addMode = false;
      this.createdAt = DateTime.fromISO(item.created);
      this.editForm = this.setUpEditForm(item);
      this.customDate = item.expiration !== null;
    }
  }

  setUpEditForm(item: Secret) {
    console.log(item);
    let value =
      item.type === 'VisibleOneTime' ? this.starsPlaceholder : item.value;
    let ownerId;
    if (this.httpService.endpoint === 'client-secret') {
      ownerId = this.clientOptions.find(x => x.text === item.clientName).value;
    } else {
      ownerId = this.apiResourceOptions.find(
        x => x.text === item.apiResourceName
      ).value;
    }
    return this.formBuilder.group({
      value: new FormControl(
        {
          value: value,
          disabled: true
        },
        Validators.required
      ),
      description: [item.description],
      type: [item.type, Validators.required],
      ownerId: [ownerId, Validators.required],
      expiresAt: new FormControl(
        { value: item.expiration ?? null, disabled: !this.customDate },
        Validators.required
      )
    });
  }

  setUpAddForm() {
    return this.formBuilder.group({
      value: [this.generateSecret(), Validators.required],
      description: [''],
      type: ['SharedSecret', Validators.required],
      ownerId: [null, Validators.required],
      expiresAt: new FormControl(
        { value: null, disabled: true },
        Validators.required
      )
    });
  }

  setExpireOption(item: Secret) {
    if (item && item.expiration) {
      this.expireSelectValue = 'Custom';
      return;
    }
    this.expireSelectValue = 'Never';
  }

  generateSecret() {
    const random = new Entropy({ bits: 170, charset: charSet64 });
    return random.string();
  }

  regenerate(items: Secret[]) {
    if (items.length > 1) {
      this.httpService
        .patchBatch('regenerate-batch', null, items)
        .subscribe(() => {
          this.getData();
        });
    } else {
      this.httpService
        .patch(items[0], items[0].id + '/regenerate')
        .subscribe(() => {
          this.getData();
        });
    }
  }

  regenerateItems() {
    this.selectedRows.forEach(x => (x.value = this.generateSecret()));
    this.regenerate(this.selectedRows);
  }

  // Checks to see if all the selected items follow the desired compatibility rules
  // Would have used break or return when one condition is not met, but "visibleOneTimeCompatible"
  // does not care about the other two.
  checkSelectedItemsCompatible() {
    this.visibleOneTimeCompatible = true;
    this.selectedItemsAreCompatible = true;
    for (let i = 1; i < this.selectedRows.length; i++) {
      // None of the selected items can be of type VisibleOneTime
      if (this.selectedRows[i]['type'] === 'VisibleOneTime') {
        this.visibleOneTimeCompatible = false;
      }

      // All selected items must share the same enabled state
      if (this.selectedRows[i].enabled !== this.selectedRows[0].enabled) {
        this.selectedItemsAreCompatible = false;
      }

      // None of the selected items can be expired
      if (
        DateTime.fromISO(this.selectedRows[i]['expiresAt']) < DateTime.utc()
      ) {
        this.selectedItemsAreCompatible = false;
      }
    }
  }

  // Controls the connection between the type of expiration, and the actual expiration date
  onExpirationChange(event: MatSelectChange) {
    this.customDate = false;
    this.expireSelectValue = event.value;
    let control = this.editForm.controls['expiresAt'];
    control.disable();

    if (!control?.value) {
      this.editForm.controls['expiresAt'].setValue(new Date());
    }
    switch (event.value) {
      case 'Never':
        control.setValue(null);
        break;
      case 'Custom':
        control.setValue(DateTime.utc().toString());
        this.customDate = true;
        control.enable();
        break;
      default:
        // noinspection TypeScriptValidateJSTypes
        control.setValue(
          this.createdAt
            .plus({ days: event.value })
            .set({ hour: 0, minute: 0, second: 0 })
            .toString()
        );
        break;
    }
  }

  onDateChange(event: MatDatepickerInputEvent<any, any>) {
    let control = this.editForm.controls['expiresAt'];
    if (control === null || control.errors) {
      return;
    }
    let selectedDate: DateTime = getDateFromLocale(event.value);
    control.setValue(selectedDate.toString());
  }

  onValueRefresh() {
    this.editForm.controls['value'].setValue(this.generateSecret());
  }

  onTypeChange(type: SecretType) {
    this.editForm.controls['type'].setValue(type);
  }

  onSubmit() {
    if (this.editForm.controls['type'].value === 'VisibleOneTime') {
      this.editForm.controls['value'].setValue('Unknown');
    }
    super.onSubmit();
  }

  // For secret owner selection during Edit / Add
  getFormDropdownOptions() {
    this.httpService
      .getOptions<BaseOption<number>>(this.currentSection)
      .subscribe(value => {
        if (this.currentSection === 'client') {
          this.clientOptions = value;
          return;
        }
        this.apiResourceOptions = value;
      });
  }

  // Next 2 functions are the code that makes possible having both Client Secrets and
  // API Resource Secret on the same page (it's ugly, I know)

  // This is the switch
  toggleSecretType() {
    this.resetPagination();
    this.filterOptionRef.value = null;
    this.currentSection =
      this.currentSection === 'client' ? 'api-resource' : 'client';
    let clientNameIndex = this.displayedColumns.indexOf('clientName');
    let apiResourceNameIndex = this.displayedColumns.indexOf('apiResourceName');

    if (this.httpService.endpoint === 'client-secret') {
      this.displayedColumns.splice(clientNameIndex, 1, 'apiResourceName');
      this.httpService.endpoint = 'api-resource-secret';
    } else {
      this.displayedColumns.splice(apiResourceNameIndex, 1, 'clientName');
      this.httpService.endpoint = 'client-secret';
    }
    this.getFormDropdownOptions();
    this.refresh(false);

    console.log(this.displayedColumns);
  }

  // This is for page initialization
  setSecretType(type: string) {
    this.httpService.endpoint =
      type === 'client' ? 'client-secret' : 'api-resource-secret';

    let clientNameIndex = this.displayedColumns.indexOf('clientName');
    let apiResourceNameIndex = this.displayedColumns.indexOf('apiResourceName');

    if (this.httpService.endpoint === 'client-secret') {
      this.displayedColumns.splice(apiResourceNameIndex, 1);
      return;
    }
    this.displayedColumns.splice(clientNameIndex, 1);
  }

  onFilterSelect(value) {
    let filter = null;
    if (value !== 'All') {
      filter = value === 'Active' ? SecretFilter.Active : SecretFilter.Expired;
    }
    super.getFilteredData(filter);
  }
}
