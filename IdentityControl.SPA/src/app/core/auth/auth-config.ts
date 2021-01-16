import { AuthConfig } from 'angular-oauth2-oidc';
import { environment } from '../../../environments/environment';

export const authConfig: AuthConfig = {
  issuer: environment.identityApi,
  clientId: 'identity_control_ng',
  responseType: 'code',
  redirectUri: window.origin + '/index.html',
  silentRefreshRedirectUri: window.origin + '/silent-refresh.html',
  scope: 'openid profile identity_control_full blog_api_full posting_api_full',
  useSilentRefresh: true,
  timeoutFactor: 0.9,
  sessionChecksEnabled: true,
  showDebugInformation: false,
  clearHashAfterLogin: false,
  nonceStateSeparator: 'semicolon'
};
