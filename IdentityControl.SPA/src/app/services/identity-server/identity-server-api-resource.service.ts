import { Injectable } from '@angular/core';
import { ServiceBase } from '../base.service';
import { environment } from '../../../environments/environment';
import { ApiResource } from '../../models/identity-server/apiResource';

@Injectable({
  providedIn: 'root'
})
export class IdentityServerApiResourceService extends ServiceBase<ApiResource> {
  endpoint = 'api-resource';
  origin = environment.identityControlApi;
}
