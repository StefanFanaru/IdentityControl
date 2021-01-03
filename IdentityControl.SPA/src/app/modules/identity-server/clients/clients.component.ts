import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import 'src/app/helpers/stringExtensions';
import { ActivatedRoute, Router } from '@angular/router';
import { IdentityServerBaseService } from '../../../services/identity-server/identity-server-base-service';
import { IdentityServerClientService } from '../../../services/identity-server/identity-server-clients.service';
import { Client, ClientChildType } from '../../../models/identity-server/client';
import { SearchService } from '../../../services/search.service';
import { SortDirection } from '../../../models/sortDirection';
import { toPascalCase } from '../../../helpers/stringExtensions';
import { environment } from '../../../../environments/environment';
import { BaseOption } from '../../../models/option';
import { Observable } from 'rxjs';

enum ClientsFilter {
  Enabled,
  Disabled,
  PkceOnly,
  WithOfflineAccess,
  WithClientSecret,
  WithBrowserAccessTokens
}

@Component({
  selector: 'app-idsrv-clients',
  templateUrl: './clients.component.html',
  styleUrls: ['../identity-server.scss', './clients.component.scss']
})
export class ClientsComponent
  extends IdentityServerBaseService<Client, IdentityServerClientService>
  implements OnInit {
  displayedColumns: string[] = [
    'selected',
    'displayName',
    'description',
    'name',
    'clientUri',
    'accessTokenLifetime',
    'created',
    'enabled'
  ];
  editForm: FormGroup;
  rows: Client[];
  submitted = false;
  apiScopeEditForm: FormGroup;
  addMode: boolean;
  pageTitle = 'Clients';
  displayNameInput: AbstractControl;
  isManagingChildren: boolean;
  childTypeManaged: ClientChildType;
  apiScopesOptions: Observable<BaseOption<string>[]>;
  apiScopesFormControl = new FormControl();
  uriPatternValidator = environment.envName == 'local' ?
    '(https?://)?(localhost+)\\:([0-9]{1,5})[/\\w .-]*/?' :
    '(https?://)?([\\da-z.-]+)\\.([a-z.]{2,6})[/\\w .-]*/?'

  // noinspection JSUnusedGlobalSymbols
  constructor(
    public httpService: IdentityServerClientService,
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

  buildForm(item: Client = null) {
    this.addMode = !item;

    if (!item) {
      this.apiScopesFormControl.setValue(null);
      this.setUpAddForm();
      this.convertDisplayName();
      this.isFormVisible = true;
    } else {
      this.httpService.get(item.id).subscribe(client => {
        this.setUpEditForm(client);
        this.convertDisplayName();
        this.httpService.getOptions<BaseOption<string>>(`api-scope/client/${this.itemSelected.id}`)
          .subscribe(clientOptions => {
            this.apiScopesFormControl.setValue(clientOptions.map(x => x.value));
            this.editForm.addControl('apiScopes', this.apiScopesFormControl);
            this.isFormVisible = true;
          });
      })
    }
  }

  setUpAddForm() {
    this.editForm = this.formBuilder.group({
      name: ['', Validators.required],
      displayName: ['', Validators.required],
      description: [''],
      requirePkce: [true, Validators.required],
      requireClientSecret: [false, Validators.required],
      allowOfflineAccess: [false, Validators.required],
      allowAccessTokensViaBrowser: [false, Validators.required],
      accessTokenLifetime: [
        10,
        Validators.compose([Validators.required, Validators.min(1)])
      ],
      clientUri: [
        'https://',
        Validators.compose([
          Validators.required,
          Validators.pattern(this.uriPatternValidator)
        ])
      ]
    });

    this.editForm.addControl('apiScopes', this.apiScopesFormControl);
  }

  setUpEditForm(item: Client) {
    this.editForm = this.formBuilder.group({
      name: [item.name, Validators.required],
      displayName: [item.displayName, Validators.required],
      description: [item.description],
      requirePkce: [item.requirePkce, Validators.required],
      requireClientSecret: [item.requireClientSecret, Validators.required],
      allowOfflineAccess: [item.allowOfflineAccess, Validators.required],
      allowAccessTokensViaBrowser: [
        item.allowAccessTokensViaBrowser,
        Validators.required
      ],
      accessTokenLifetime: [
        item.accessTokenLifetime,
        Validators.compose([Validators.required, Validators.min(1)])
      ],
      clientUri: [
        item.clientUri,
        Validators.compose([
          Validators.required,
          Validators.pattern(this.uriPatternValidator)
        ])
      ]
    });
  }

  onFilterSelect(value) {
    super.getFilteredData(value === 'All' ? null : value);
  }

  toPscCase(value: string) {
    console.log(toPascalCase(value));
    return toPascalCase(value);
  }

  goToSecrets() {
    this.router.navigate(['../secrets'], {
      relativeTo: this.route,
      queryParams: { relation: 'client', id: this.itemSelected.id }
    });
  }

  goToApiScopes() {
    this.router.navigate(['../api-scopes'], {
      relativeTo: this.route,
      queryParams: { relation: 'client', id: this.itemSelected.id }
    });
  }

  toggleManagePanel(childType: ClientChildType = null) {
    this.childTypeManaged = childType;
    this.isManagingChildren = !this.isManagingChildren;
  }
}
