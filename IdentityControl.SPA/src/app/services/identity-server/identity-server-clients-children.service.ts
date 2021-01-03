import { Injectable } from '@angular/core';
import { ServiceBase } from '../base.service';
import { environment } from '../../../environments/environment';
import { ClientChild, ClientChildType } from '../../models/identity-server/client';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class IdentityServerClientsChildrenService extends ServiceBase<ClientChild> {
  endpoint = 'client';
  origin = environment.identityControlApi;

  getChildren(
    clientId: string,
    childType: ClientChildType
  ): Observable<ClientChild[]> {
    return this.getAny<ClientChild[]>(clientId, `children/${childType}`);
  }

  deleteChild(clientId: string, childType: ClientChildType, childId: number) {
    return this.delete(`${clientId}/children/${childType}/delete/${childId}`);
  }

  searchChildren(
    clientId: string,
    childType: ClientChildType,
    searchTerm: string
  ): Observable<ClientChild[]> {
    return this.getAny<ClientChild[]>(
      clientId,
      `children/${childType}/${searchTerm}`
    );
  }

  assignChild(
    clientId: string,
    request: {value: string; type: ClientChildType}
  ) {
    return this.postAny(request, `${clientId}/children/assignment`);
  }
}
