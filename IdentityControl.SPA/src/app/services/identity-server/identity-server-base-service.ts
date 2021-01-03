import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { AbstractControl, FormGroup } from '@angular/forms';
import { ServiceBase } from '../base.service';
import { BaseIdentityModel } from '../../models/baseIdentityModel';
import { DateTime } from 'luxon';
import { PageEvent } from '@angular/material/paginator';
import { ActivatedRoute, Router } from '@angular/router';
import { toSnakeCase } from '../../helpers/stringExtensions';
import { SortDirection } from '../../models/sortDirection';
import { Sort } from '@angular/material/sort';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { SearchService } from '../search.service';
import { IdentityTableQuery } from '../../models/identity-server/identityTableQuery';
import { PageOf } from '../../models/pageOf';
import { DialogService } from '../dialog.service';

export abstract class IdentityServerBaseService<TItem extends BaseIdentityModel,
  ItemService extends ServiceBase<TItem>> {
  protected constructor() {
  }

  dialogService: DialogService
  httpService: ItemService;
  router: Router;
  route: ActivatedRoute;
  searchService: SearchService;
  rows: TItem[];
  rowsLoaded: Subject<boolean> = new BehaviorSubject<boolean>(false);
  selectedRows: TItem[] = [];
  editForm: FormGroup;
  addMode: boolean;
  isFormVisible: boolean;
  submitResponse: Observable<any>;
  selectMultiple = false;
  allSelected: boolean;
  selectedItemsAreCompatible: boolean;
  visibleOneTimeCompatible = true;
  dateTime = DateTime;
  displayNameInput: AbstractControl;
  totalItems: number;
  filterSelectHasValue: boolean;
  currentSection: string;
  initQueryParams = true;
  queryParams: IdentityTableQuery = {
    filterType: null,
    pageIndex: 0,
    pageSize: 15,
    sortColumn: null,
    sortDirection: SortDirection.Dsc,
    searchTerm: null,
    id: null,
    relation: null
  };

  get itemSelected() {
    return this.selectedRows[0];
  }

  set itemSelected(value) {
    this.selectedRows[0] = value;
  }

  initialize() {
    this.resetPagination();
    this.searchService.$searchInput
      .pipe(
        debounceTime(500),
        distinctUntilChanged(),
        tap(term => {
          this.queryParams.searchTerm = term === '' ? null : term;
          if (this.queryParams.pageIndex !== 0) {
            this.queryParams.pageIndex = 0;
          }
          this.refresh();
        })
      )
      .subscribe();
  }

  refresh(reselectRows = true) {
    if (this.initQueryParams) {
      this.initQueryParams = false;
      this.queryParams = Object.assign(
        {},
        this.route.snapshot.queryParams as IdentityTableQuery
      );
      this.queryParams.pageIndex ??= 0;
      this.queryParams.pageSize ??= 15;
      this.queryParams.sortDirection ??= SortDirection.Dsc;
    }

    if (this.currentSection) {
      this.queryParams.relation = this.currentSection; // cheated the type a bit here...
    }

    this.router
      .navigate([], {
        relativeTo: this.route,
        queryParams: this.queryParams
      })
      .then(() => this.getData(reselectRows));
  }

  resetPagination() {
    this.queryParams = {
      filterType: null,
      pageIndex: 0,
      pageSize: 15,
      sortColumn: null,
      sortDirection: SortDirection.Dsc,
      searchTerm: null,
      id: null,
      relation: null
    };
  }

  ////////// FORM MANIPULATION //////////

  buildForm(item: TItem = null) {
  }

  openForm(addMode: boolean) {
    this.buildForm(addMode ? null : this.itemSelected);
  }

  ////////// TABLE MANIPULATION //////////

  selectRow(item: TItem) {
    if (item.isReadOnly) {
      return;
    }
    if (this.selectMultiple) {
      if (item.selected) {
        // Filtering out the row that is now unselected
        this.selectedRows = this.selectedRows.filter(x => x.id !== item.id);
      } else {
        this.selectedRows.push(item);
      }
      item.selected = !item.selected; // Toggle selection
      this.checkSelectedItemsCompatible();
    } else {
      // No multiple selection available
      if (!item.selected) {
        // Unselecting all rows because we don't know witch one was selected beforehand
        this.rows.forEach(x => (x.selected = false));

        // Select the desired row
        item.selected = true;
        this.selectedRows = [item];
      }
    }
  }

  changeAllSelectionStates(value: boolean) {
    this.allSelected = value;
    this.rows.forEach(x => (x.selected = value));
    this.selectedRows = value ? this.rows : [];
    this.checkSelectedItemsCompatible();
  }

  selectMultipleToggle() {
    this.selectMultiple = !this.selectMultiple;
    if (!this.selectMultiple) {
      this.changeAllSelectionStates(false);
    }
  }

  // Checks to see if all the selected items follow the desired compatibility rules
  checkSelectedItemsCompatible() {
    this.visibleOneTimeCompatible = true;
    this.selectedItemsAreCompatible = true;
    for (let i = 1; i < this.selectedRows.length; i++) {
      // All selected items must share the same enabled state
      if (this.selectedRows[i].enabled !== this.selectedRows[0].enabled) {
        this.selectedItemsAreCompatible = false;
      }
    }
  }

  throwIfReadOnly(item: TItem) {
    if (item.isReadOnly) {
      throw new Error('Tried to change readonly item!');
    }
  }

  ////////// HTTP CALLS //////////

  getData(reselectRows = true) {
    console.log(this.queryParams);
    let data: Observable<PageOf<TItem>>;

    if (this.queryParams.id) {
      data = this.httpService.getTableListOf(
        this.queryParams,
        `${this.queryParams.relation}/${this.queryParams.id}`
      );
    } else {
      data = this.httpService.getTableList(this.queryParams);
    }

    data.subscribe(response => {
      this.rows = response.pageData;
      this.totalItems = response.totalItems;
      this.selectedItemsAreCompatible = true;
      // Re-selecting the rows that were selected before the pagination event
      if (response.pageData.length > 0 && reselectRows) {
        this.selectedRows = this.rows.filter(row =>
          this.selectedRows.map(x => x.id).includes(row.id)
        );
        this.selectedRows.forEach(x => (x.selected = true));
      } else {
        this.selectedRows = [];
      }
      this.rowsLoaded.next(true);
    });
  }

  insert(item: TItem) {
    this.submitResponse = this.httpService.post(item);
  }

  update(item: TItem) {
    this.submitResponse = this.httpService.patch(item, item.id);
  }

  delete(itemName: string) {
    this.dialogService.openConfirmationDialog(`Delete ${itemName}`, `Are you sure you want to delete "${itemName}"?`, true,
      () => {
        if (this.selectMultiple) {
          this.selectedRows.forEach(x => this.throwIfReadOnly(x));
          let ids = this.selectedRows.map(x => x.id);
          this.httpService.patchBatch('delete-batch', ids).subscribe(() => {
            this.allSelected = false;
            this.getData();
          });
        } else {
          this.throwIfReadOnly(this.itemSelected);
          this.httpService
            .delete(this.itemSelected.id)
            .subscribe(() => this.getData());
        }
      })
  }

  changeEnabledState() {
    if (this.selectMultiple && this.selectedItemsAreCompatible) {
      this.selectedRows.forEach(x => this.throwIfReadOnly(x));
      let ids = this.selectedRows.map(x => x.id);
      this.httpService
        .patchBatch(
          this.selectedRows[0].enabled ? 'disable-batch' : 'enable-batch',
          ids
        )
        .subscribe(() => {
          this.allSelected = false;
          this.getData();
        });
    } else {
      this.throwIfReadOnly(this.itemSelected);
      this.httpService
        .patchAny(
          null,
          (this.itemSelected.enabled ? 'disable/' : 'enable/') +
          this.itemSelected.id
        )
        .subscribe(() => this.getData());
    }
  }

  onSubmit() {
    if (this.editForm.valid) {
      let item = Object.assign({}, this.editForm.getRawValue());
      item.id = this.itemSelected?.id;
      this.addMode ? this.insert(item) : this.update(item);
      this.handleSubmitResponse();
      this.closeEditPanel();
    }
  }

  onTablePageChange(event: PageEvent) {
    this.queryParams.pageIndex = event.pageIndex;
    this.queryParams.pageSize = event.pageSize;
    this.refresh();
  }

  handleSubmitResponse() {
    this.submitResponse.subscribe(() => {
      this.addMode = false;
      this.getData();
    });
  }

  closeEditPanel() {
    this.addMode = false;
    this.isFormVisible = false;
  }

  convertDisplayName() {
    this.displayNameInput = this.editForm.controls['displayName'];
    let nameInput = this.editForm.controls['name'];
    if (!nameInput.value) {
      this.displayNameInput.valueChanges.subscribe((value: string) => {
        this.editForm.controls['name'].setValue(toSnakeCase(value));
      });
    }
  }

  getFilteredData(filter: number) {
    this.filterSelectHasValue = true;
    this.queryParams.filterType = filter;
    this.refresh();
  }

  sortData(sort: Sort) {
    // if (!sort.active || sort.direction === '') {
    //   return;
    // }
    this.queryParams.sortColumn = sort.active.firstCharUpper();
    this.queryParams.sortDirection =
      sort.direction === 'asc' ? SortDirection.Asc : SortDirection.Dsc;

    this.refresh();
  }
}
