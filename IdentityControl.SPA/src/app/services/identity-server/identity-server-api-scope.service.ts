import { Injectable } from '@angular/core';
import { ServiceBase } from '../base.service';
import { environment } from '../../../environments/environment';
import { ApiScope } from '../../models/identity-server/apiScope';

@Injectable({
  providedIn: 'root'
})
export class IdentityServerApiScopeService extends ServiceBase<ApiScope> {
  endpoint = 'api-scope';
  origin = environment.identityControlApi;
}
