import { Component, OnInit } from '@angular/core';
import 'src/app/helpers/stringExtensions';
import { Router } from '@angular/router';
import { UserService } from '../../services/users/user.service';

@Component({
  selector: 'app-management-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {
  constructor(private userService: UserService, public router: Router) {
  }

  ngOnInit(): void {
  }

  // updateFilter(event) {
  //   const val = event.target.value.toLowerCase();
  //
  //   // filter our data
  //   const temp = this.temp.filter(function (d) {
  //     return d.name.toLowerCase().indexOf(val) !== -1 || !val;
  //   });
  //
  //   // update the rows
  //   this.rows = temp;
  //   // Whenever the filter changes, always go back to the first page
  //   this.table.offset = 0;
  // }
}
