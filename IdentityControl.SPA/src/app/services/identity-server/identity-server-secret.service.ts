import { Injectable } from '@angular/core';
import { ServiceBase } from '../base.service';
import { environment } from '../../../environments/environment';
import { Secret } from '../../models/identity-server/secret';

@Injectable({
  providedIn: 'root'
})
export class IdentityServerSecretService extends ServiceBase<Secret> {
  endpoint = 'client-secret';
  origin = environment.identityControlApi;
}
