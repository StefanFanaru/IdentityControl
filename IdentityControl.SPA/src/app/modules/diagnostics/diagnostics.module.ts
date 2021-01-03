import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../shared/shared.module';
import { DiagnosticsAuthComponent } from './dignostics-auth/diagnostics-auth.component';
import { DiagnosticsProfileComponent } from './diagnostics-profile/diagnostics-profile.component';
import { NgxPermissionsModule } from 'ngx-permissions';

@NgModule({
  declarations: [DiagnosticsAuthComponent, DiagnosticsProfileComponent],
  imports: [
    CommonModule,
    RouterModule.forChild([
      { path: '', redirectTo: 'auth' },
      { path: 'auth', component: DiagnosticsAuthComponent },
      { path: 'profile', component: DiagnosticsProfileComponent }
    ]),
    SharedModule,
    NgxPermissionsModule.forChild()
  ]
})
export class DiagnosticsModule {
}
