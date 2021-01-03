import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import 'src/app/helpers/stringExtensions';
import { ActivatedRoute, Router } from '@angular/router';
import { IdentityServerBaseService } from '../../../services/identity-server/identity-server-base-service';
import { IdentityServerApiScopeService } from '../../../services/identity-server/identity-server-api-scope.service';
import { ApiScope } from '../../../models/identity-server/apiScope';
import { SearchService } from '../../../services/search.service';
import { SortDirection } from '../../../models/sortDirection';
import { DialogService } from '../../../services/dialog.service';

enum ApiScopesFilter {
  Enabled,
  Disabled
}

@Component({
  selector: 'app-api-scopes',
  templateUrl: './api-scopes.component.html',
  styleUrls: ['../identity-server.scss', './api-scopes.component.scss']
})
export class ApiScopesComponent
  extends IdentityServerBaseService<ApiScope, IdentityServerApiScopeService>
  implements OnInit {
  displayedColumns: (keyof ApiScope)[] = [
    'selected',
    'displayName',
    'description',
    'name',
    'enabled'
  ];
  editForm: FormGroup;
  rows: ApiScope[];
  submitted = false;
  apiScopeEditForm: FormGroup;
  addMode: boolean;
  pageTitle = 'API Scopes';
  displayNameInput: AbstractControl;

  // noinspection JSUnusedGlobalSymbols
  constructor(
    public httpService: IdentityServerApiScopeService,
    public formBuilder: FormBuilder,
    public searchService: SearchService,
    public route: ActivatedRoute,
    public router: Router,
    public dialogService: DialogService
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
  }

  buildForm(item: ApiScope = null) {
    this.isFormVisible = true;
    this.addMode = !item;
    this.editForm = this.formBuilder.group({
      name: [this.addMode ? '' : item.name, Validators.required],
      displayName: [this.addMode ? '' : item.displayName, Validators.required],
      description: [this.addMode ? '' : item.description]
    });
    this.convertDisplayName();
  }

  onFilterSelect(value) {
    let filter = null;
    if (value !== 'All') {
      filter =
        value === 'Enabled'
          ? ApiScopesFilter.Enabled
          : ApiScopesFilter.Disabled;
    }
    super.getFilteredData(filter);
  }
}
