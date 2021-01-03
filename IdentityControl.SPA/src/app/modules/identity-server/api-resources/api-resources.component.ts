import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import 'src/app/helpers/stringExtensions';
import { ActivatedRoute, Router } from '@angular/router';
import { IdentityServerBaseService } from '../../../services/identity-server/identity-server-base-service';
import { ApiResource } from '../../../models/identity-server/apiResource';
import { IdentityServerApiResourceService } from '../../../services/identity-server/identity-server-api-resource.service';
import { SearchService } from '../../../services/search.service';
import { SortDirection } from '../../../models/sortDirection';
import { Observable } from 'rxjs';
import { BaseOption } from '../../../models/option';

enum ApiResourceFilter {
  Enabled,
  Disabled
}

@Component({
  selector: 'app-api-resources',
  templateUrl: './api-resources.component.html',
  styleUrls: ['../identity-server.scss', './api-resources.component.scss']
})
export class ApiResourcesComponent
  extends IdentityServerBaseService<ApiResource,
    IdentityServerApiResourceService>
  implements OnInit {
  displayedColumns: (keyof ApiResource)[] = [
    'selected',
    'displayName',
    'description',
    'name',
    'created',
    'updated',
    'enabled'
  ];
  editForm: FormGroup;
  rows: ApiResource[];
  submitted = false;
  apiScopeEditForm: FormGroup;
  addMode: boolean;
  pageTitle = 'API Resources';
  apiScopesOptions: Observable<BaseOption<string>[]>;
  apiScopesFormControl = new FormControl();

  constructor(
    public httpService: IdentityServerApiResourceService,
    public formBuilder: FormBuilder,
    public searchService: SearchService,
    public route: ActivatedRoute,
    public router: Router
  ) {
    super();
    this.queryParams.sortColumn = 'DisplayName';
    this.queryParams.sortDirection = SortDirection.Asc;
  }

  get itemSelected() {
    return this.selectedRows[0];
  }

  set itemSelected(value) {
    this.selectedRows[0] = value;
  }

  ngOnInit(): void {
    this.initialize();
    this.apiScopesOptions = this.httpService.getOptions<BaseOption<string>>(`api-scope`)
  }

  buildForm(item: ApiResource = null) {
    this.addMode = !item;
    if (!item) {
      this.apiScopesFormControl.setValue(null);
      this.setUpAddForm();
      this.convertDisplayName();
      this.isFormVisible = true;
    } else {
      this.httpService.getOptions<BaseOption<string>>(`api-scope/api-resource/${this.itemSelected.id}`)
        .subscribe(apiScopeOptions => {
          this.setUpEditForm(item);
          this.convertDisplayName();
          this.apiScopesFormControl.setValue(apiScopeOptions.map(x => x.value));
          this.editForm.addControl('apiScopes', this.apiScopesFormControl);
          this.isFormVisible = true;
        })
    }
  }

  setUpAddForm() {
    this.editForm = this.formBuilder.group({
      name: ['', Validators.required],
      displayName: ['', Validators.required],
      description: ['']
    });
  }

  setUpEditForm(item: ApiResource) {
    this.editForm = this.formBuilder.group({
      name: [item.name, Validators.required],
      displayName: [item.displayName, Validators.required],
      description: [item.description]
    });
  }


  onFilterSelect(value) {
    let filter = null;
    if (value !== 'All') {
      filter =
        value === 'Enabled'
          ? ApiResourceFilter.Enabled
          : ApiResourceFilter.Disabled;
    }
    super.getFilteredData(filter);
  }
}
