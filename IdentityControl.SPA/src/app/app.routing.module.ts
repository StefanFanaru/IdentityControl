import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UsersComponent } from './modules/users/users.component';

const routes: Routes = [
  { path: '', redirectTo: 'control/user', pathMatch: 'full' },
  {
    path: 'diagnostics',
    loadChildren: () =>
      import('./modules/diagnostics/diagnostics.module').then(
        m => m.DiagnosticsModule
      )
  },
  {
    path: 'users',
    loadChildren: () =>
      import('./modules/users/users.module').then(
        m => m.UsersModule
      )
  },
  {
    path: 'identity-server',
    loadChildren: () =>
      import('./modules/identity-server/identity-server.module').then(
        m => m.IdentityServerModule
      )
  },

  { path: '**', component: UsersComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
