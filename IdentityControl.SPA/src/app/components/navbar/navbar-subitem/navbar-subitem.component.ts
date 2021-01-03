import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'navbar-subitem',
  templateUrl: './navbar-subitem.component.html',
  styleUrls: ['./navbar-subitem.component.scss']
})
export class NavbarSubitemComponent implements OnInit {
  @Input() item: SubItem;
  @Input() isExpanded: boolean;

  constructor() {
  }

  ngOnInit(): void {
  }
}

export interface SubItem {
  text: string;
  link: string;
  matIcon: string;
  matIconClass: string;
  fontawesomeIcon: string;
}
