// import { AfterViewInit, Component, OnInit } from '@angular/auth';
// import { AuthService } from "../../services/auth.service";
// import { map } from "rxjs/operators";
// import { OAuthService } from "angular-oauth2-oidc";
// import { ActivatedRoute } from "@angular/router";
// import { authCodeFlowConfig } from "../../modules/authentication/authConfig";
//
// @Component({
//   selector: 'app-home',
//   templateUrl: './home.component.html',
//   styleUrls: ['./home.component.scss']
// })
// export class HomeComponent implements OnInit, AfterViewInit {
//   title = 'Home'
//   claims: string;
//   managementSecret: any;
//   isLogged: boolean;
//   login: false;
//
//   constructor(public authService: AuthService, public oauthService: OAuthService,  private route: ActivatedRoute,) {
//     this.oauthService.loadDiscoveryDocumentAndTryLogin().then(r => {
//       this
//         .oauthService
//         .silentRefresh()
//         .then(info => {
//           this.isLogged = true;
//           console.debug('refresh ok', info)
//         } )
//         .catch(err => console.error('refresh error', err));
//     });
//     // this.oauthService.loadDiscoveryDocumentAndTryLogin().then(() =>{
//     //
//     //     if (!this.oauthService.hasValidIdToken() || !this.oauthService.hasValidAccessToken()) {
//     //       this.oauthService.initCodeFlow();
//     //     }
//     // });
//   }
//
//   ngOnInit(): void {
//   }
//
//   get userName(): string {
//     const claims = this.oauthService.getIdentityClaims();
//     if (!claims) return null;
//     return claims['given_name'];
//   }
//
//   get idToken(): string {
//     return this.oauthService.getIdToken();
//   }
//
//   get accessToken(): string {
//     return this.oauthService.getAccessToken();
//   }
//
//   getManagementSecret(){
//      this.authService.callManagementApi().subscribe((data: string) => this.managementSecret = data);
//   }
//
//   refreshMyToken(){
//     this.oauthService.refreshToken().then(x => console.log(x.refresh_token))
//   }
//
//   init() {
//
//   }
//
//   ngAfterViewInit(): void {
//
//   }
// }
