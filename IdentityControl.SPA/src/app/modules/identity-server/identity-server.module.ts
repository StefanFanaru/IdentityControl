import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiScopesComponent } from './api-scopes/api-scopes.component';
import { RouterModule } from '@angular/router';
import { NgxPermissionsModule } from 'ngx-permissions';
import { SecretsComponent } from './secrets/secrets.component';
import { ApiResourcesComponent } from './api-resources/api-resources.component';
import { ClientsComponent } from './clients/clients.component';
import { ClientChildrenComponent } from './clients/client-children/client-children.component';
import { IdentityServerApiScopeService } from '../../services/identity-server/identity-server-api-scope.service';
import { IdentityServerApiResourceService } from '../../services/identity-server/identity-server-api-resource.service';
import { IdentityServerClientsChildrenService } from '../../services/identity-server/identity-server-clients-children.service';
import { IdentityServerClientService } from '../../services/identity-server/identity-server-clients.service';
import { IdentityServerSecretService } from '../../services/identity-server/identity-server-secret.service';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [
    ApiScopesComponent,
    SecretsComponent,
    ApiResourcesComponent,
    ClientsComponent,
    ClientChildrenComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild([
      { path: '', redirectTo: 'api-scopes' },
      { path: 'api-scopes', component: ApiScopesComponent },
      { path: 'api-resources', component: ApiResourcesComponent },
      { path: 'secrets', component: SecretsComponent },
      { path: 'clients', component: ClientsComponent }
    ]),
    SharedModule,
    NgxPermissionsModule.forChild()
  ],
  providers: [
    IdentityServerApiScopeService,
    IdentityServerSecretService,
    IdentityServerApiResourceService,
    IdentityServerClientService,
    IdentityServerClientsChildrenService
  ]
})
export class IdentityServerModule {
}
