import { Component, Input, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs/operators';
import { SubItem } from '../navbar-subitem/navbar-subitem.component';

@Component({
  selector: 'navbar-item',
  templateUrl: './navbar-item.component.html',
  styleUrls: ['./navbar-item.component.scss']
})
export class NavbarItemComponent implements OnInit {

  private _isExpanded;
  showArrows: boolean;
  animateArrows: boolean;
  opened: boolean;
  subItemActive: boolean;
  @Input() item: NavbarItem;
  @Input() subItems: SubItem[];

  get isExpanded(): boolean {
    return this._isExpanded;
  }

  @Input() set isExpanded(value: boolean) {
    this._isExpanded = value;
    this.animateArrows = false;
    setTimeout(() => {
      this.showArrows = value;
    }, 200);
  }

  constructor(private router: Router) {
    router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        this.subItemActive =
          this.item?.activeLinks.find(x =>
            event.urlAfterRedirects.includes(x)
          ) !== undefined;
        if (!this.opened) {
          this.opened = this.subItemActive;
        }
      });
  }

  ngOnInit(): void {
  }

  onItemClick() {
    this.animateArrows = true;
    this.opened = !this.opened;
    if (!this.subItems) {
      this.router.navigateByUrl(this.item.link)
    }
  }
}

export interface NavbarItem {
  text: string;
  link: string;
  matIcon: string;
  matIconClass: string;
  fontawesomeIcon: string;
  activeLinks: string[];
}
