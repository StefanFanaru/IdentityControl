import { Injectable } from '@angular/core';
import { ServiceBase } from '../base.service';
import { environment } from '../../../environments/environment';
import { Client } from '../../models/identity-server/client';

@Injectable({
  providedIn: 'root'
})
export class IdentityServerClientService extends ServiceBase<Client> {
  endpoint = 'client';
  origin = environment.identityControlApi;
}
