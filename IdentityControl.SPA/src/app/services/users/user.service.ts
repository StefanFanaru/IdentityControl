import { Injectable } from '@angular/core';
import { ServiceBase } from '../base.service';
import { ApplicationUser } from '../../models/identity-server/applicationUser';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService extends ServiceBase<ApplicationUser> {
  endpoint = 'user';
  origin = environment.identityControlApi;
}
