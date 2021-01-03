import { AfterViewInit, Component, Input, OnInit } from '@angular/core';
import { SubItem } from './navbar-subitem/navbar-subitem.component';
import { NavbarItem } from './navbar-item/navbar-item.component';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit, AfterViewInit {
  @Input() item: NavbarItem;
  @Input() subItems: SubItem[];
  opened: boolean;
  subItemActive: boolean;
  private _isExpanded;

  get isExpanded(): boolean {
    return this._isExpanded;
  }

  set isExpanded(value: boolean) {
    setTimeout(() => {
      this._isExpanded = value;
    }, 180);
  }

  identityServerSubItems: SubItem[] = [
    {
      text: 'API Resources',
      link: '/identity-server/api-resources',
      matIcon: null,
      matIconClass: null,
      fontawesomeIcon: 'fas fa-box-open'
    },
    {
      text: 'API Scopes',
      link: '/identity-server/api-scopes',
      matIcon: null,
      matIconClass: null,
      fontawesomeIcon: 'fab fa-quinscape'
    },
    {
      text: 'Clients',
      link: '/identity-server/clients',
      matIcon: null,
      matIconClass: null,
      fontawesomeIcon: 'fas fa-desktop'
    },
    {
      text: 'Secrets',
      link: '/identity-server/secrets',
      matIcon: null,
      matIconClass: null,
      fontawesomeIcon: 'fas fa-key'
    }
  ];

  diagnosticsSubItems: SubItem[] = [
    {
      text: 'Authentication',
      link: '/diagnostics/auth',
      matIcon: null,
      matIconClass: null,
      fontawesomeIcon: 'fas fa-unlock-alt'
    },
    {
      text: 'Profile',
      link: '/diagnostics/profile',
      matIcon: null,
      matIconClass: null,
      fontawesomeIcon: 'fas fa-user'
    }
  ];

  users: NavbarItem = {
    text: 'Users',
    link: '/users',
    matIcon: null,
    matIconClass: null,
    fontawesomeIcon: 'fas fa-users',
    activeLinks: ['/users']
  };

  identityServer: NavbarItem = {
    text: 'Identity Server',
    link: '/identity-server',
    matIcon: 'dns',
    matIconClass: null,
    fontawesomeIcon: null,
    activeLinks: this.identityServerSubItems.map(x => x.link)
  };

  diagnostics: NavbarItem = {
    text: 'Diagnostics',
    link: '/diagnostics',
    matIcon: null,
    matIconClass: null,
    fontawesomeIcon: 'fas fa-stethoscope',
    activeLinks: this.diagnosticsSubItems.map(x => x.link)
  };

  constructor() {
    this._isExpanded = JSON.parse(localStorage.getItem('nav-expanded'));
    console.log();
  }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      document.body.className = '';
    }, 2000);
  }

  onExpandControlClick() {
    localStorage.setItem('nav-expanded', JSON.stringify(!this._isExpanded));
    this.isExpanded = !this.isExpanded;
  }
}
