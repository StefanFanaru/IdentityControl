import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { NgxPermissionsModule } from 'ngx-permissions';
import { UsersComponent } from './users.component';
import { SharedModule } from '../shared/shared.module';
import { UserService } from '../../services/users/user.service';

@NgModule({
  declarations: [UsersComponent],
  imports: [
    CommonModule,
    RouterModule.forChild([
      { path: '', component: UsersComponent }
    ]),
    SharedModule,
    NgxPermissionsModule.forChild()
  ],
  providers: [UserService]
})
export class UsersModule {
}
