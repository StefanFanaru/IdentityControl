import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ClientChild, ClientChildType } from '../../../../models/identity-server/client';
import { IdentityServerClientsChildrenService } from '../../../../services/identity-server/identity-server-clients-children.service';
import { BehaviorSubject, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { BaseOption } from '../../../../models/option';
import { DialogService } from '../../../../services/dialog.service';

@Component({
  selector: 'client-children',
  templateUrl: './client-children.component.html',
  styleUrls: ['../../identity-server.scss', './client-children.component.scss']
})
export class ClientChildrenComponent implements OnInit {
  @Input() clientId: string;
  @Input() childType: ClientChildType;
  @Output() closePanel = new EventEmitter<boolean>();
  rows: ClientChild[];
  title: string;
  displayedColumns = ['value', 'controls'];
  searchInputEvent: Subject<string> = new BehaviorSubject<string>('');
  searchHasValue: boolean;
  searchPlaceholder: string;
  searchPlaceholderText: string;
  showSearchInputBorder: boolean;
  valueToAdd: string;
  grantTypeOptions: BaseOption<string>[] = [
    {
      text: 'AuthorizationCode',
      value: 'authorization_code'
    },
    {
      text: 'ClientCredentials',
      value: 'client_credentials'
    },
    {
      text: 'Implicit',
      value: 'implicit'
    },
    {
      text: 'Hybrid',
      value: 'hybrid'
    },
    {
      text: 'ResourceOwnerPassword',
      value: 'password'
    },
    {
      text: 'DeviceFlow',
      value: 'urn:ietf:params:oauth:grant-type:device_code'
    }
  ];
  @ViewChild('searchInput') searchInputRef: ElementRef;
  filterSelectHasValue: boolean;

  constructor(
    private clientChildrenService: IdentityServerClientsChildrenService,
    private dialogService: DialogService
  ) {
  }

  ngOnInit(): void {
    this.title = ClientChildType[this.childType].formatKey();
    this.searchPlaceholder = 'Search for a ' + this.title.toLocaleLowerCase();
    this.searchPlaceholderText = this.searchPlaceholder;
    this.grantTypeOptions.forEach(x => (x.text = x.text.formatKey()));
    this.refresh();

    this.searchInputEvent
      .pipe(
        debounceTime(500),
        distinctUntilChanged(),
        switchMap((term: string) => {
          if (term !== ' ') {
            return this.clientChildrenService.searchChildren(
              this.clientId,
              this.childType,
              term
            );
          }
          return [];
        })
      )
      .subscribe(value => (this.rows = value));
  }

  onPanelClose() {
    this.closePanel.emit();
  }

  deleteChild(childId: number) {
    this.dialogService.openConfirmationDialog(`Delete ${ClientChildType[this.childType].formatKey()}`,
      `Are you sure you want to delete "${this.rows.find(x => x.id === childId).value}"?`, true, () => {
        this.clientChildrenService
          .deleteChild(this.clientId, this.childType, childId)
          .subscribe(() => this.refresh());
      })
  }

  refresh() {
    this.clientChildrenService
      .getChildren(this.clientId, this.childType)
      .subscribe(x => (this.rows = x));
  }

  clearSearchInput() {
    this.searchInputRef.nativeElement.value = '';
    this.onSearchBlur();
    this.refresh();
  }

  onSearchBlur() {
    if (!this.searchInputRef.nativeElement.value) {
      this.searchHasValue = false;
      this.searchPlaceholder = this.searchPlaceholderText;
    }
    this.showSearchInputBorder = false;
  }

  onSearchInputFocus() {
    this.searchHasValue = true;
    this.showSearchInputBorder = true;
    this.searchPlaceholder = '';
  }

  addValue() {
    this.clientChildrenService
      .assignChild(this.clientId, {
        value: this.valueToAdd,
        type: this.childType
      })
      .subscribe(() => this.refresh());
  }

  onFilterSelect(value: any) {
    this.filterSelectHasValue = true;
    this.valueToAdd = value;
  }
}
