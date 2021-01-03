import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Guid } from '../helpers/guid';
import { GetPaginatedResponse } from '../models/getPaginatedResponse';
import { PageOf } from '../models/pageOf';
import { IdentityTableQuery } from '../models/identity-server/identityTableQuery';

@Injectable({
  providedIn: 'root'
})
export abstract class ServiceBase<TEntity> {
  abstract endpoint: string;
  abstract origin: string;
  protected http: HttpClient;

  constructor(@Inject(HttpClient) http: any) {
    this.http = http;
  }

  get(id: string, route: string = null, params: {} = {}): Observable<TEntity> {
    return this.http.get<TEntity>(this.getApiUrl(route, id), {
      // tslint:disable-next-line:object-literal-shorthand
      params: params,
      headers: this.buildHeaders()
    });
  }

  async getAsync(
    id: string,
    route: string = null,
    params: {} = {}
  ): Promise<TEntity> {
    const promise = this.http
      .get<TEntity>(this.getApiUrl(route, id), {
        params: params,
        headers: this.buildHeaders()
      })
      .toPromise();
    promise.catch((error: any) => {
      this.handleError(error);
    });
    return await promise;
  }

  getAny<T>(
    id: string = null,
    route: string = null,
    params: {} = {}
  ): Observable<T> {
    return this.http.get<T>(this.getApiUrl(route, id), {
      params: params,
      headers: this.buildHeaders()
    });
  }

  getPaginated<T>(
    pageIndex: number,
    pageSize: number,
    params: {} = {}
  ): Observable<GetPaginatedResponse<T>> {
    let route = `paginated?pageIndex=${pageIndex}&pageSize=${pageSize}`;
    return this.http.get<GetPaginatedResponse<T>>(this.getApiUrl(route), {
      params: params,
      headers: this.buildHeaders()
    });
  }

  getTableList(request: IdentityTableQuery): Observable<PageOf<TEntity>> {
    return this.http.get<PageOf<TEntity>>(
      this.getApiUrl(this.buildTableListQuery(request), 'table-list'),
      {
        headers: this.buildHeaders()
      }
    );
  }

  getTableListOf(
    request: IdentityTableQuery,
    route: string
  ): Observable<PageOf<TEntity>> {
    return this.http.get<PageOf<TEntity>>(
      this.getApiUrl(`table-list/${route}` + this.buildTableListQuery(request)),
      {
        headers: this.buildHeaders()
      }
    );
  }

  buildTableListQuery(request: IdentityTableQuery): string {
    let queryParams = `?pageIndex=${request.pageIndex}&pageSize=${request.pageSize}`;
    queryParams += request.sortColumn
      ? `&sortColumn=${request.sortColumn}`
      : '';
    queryParams += request.filterType
      ? `&filterType=${request.filterType}`
      : '';
    queryParams +=
      request.sortDirection !== null // because it can come as 0
        ? `&sortDirection=${request.sortDirection}`
        : '&sortDirection=Dsc';
    queryParams += request.searchTerm
      ? `&searchTerm=${request.searchTerm}`
      : '';
    queryParams += request.relation ? `&relation=${request.relation}` : '';
    queryParams += request.id ? `&id=${request['id']}` : '';
    return queryParams;
  }

  getOptions<TOption>(endpoint: string = null): Observable<TOption[]> {
    return this.http.get<TOption[]>(
      `${this.origin}/${endpoint ?? this.endpoint}/options`,
      {
        headers: this.buildHeaders()
      }
    );
  }

  query(route: string = null, params: {} = {}): Observable<TEntity[]> {
    return this.http.get<TEntity[]>(this.getApiUrl(route), {
      params: params,
      headers: this.buildHeaders()
    });
  }

  queryAny<TModel>(route: string, params: {} = {}): Observable<TModel[]> {
    return this.http.get<TModel[]>(this.getApiUrl(route), {
      params: params,
      headers: this.buildHeaders()
    });
  }

  async queryRouteAsync(
    route: string = null,
    params: {} = {}
  ): Promise<TEntity[]> {
    const promise = this.http
      .get<TEntity[]>(this.getApiUrl(route), {
        params: params,
        headers: this.buildHeaders()
      })
      .toPromise();
    promise.catch((error: any) => {
      this.handleError(error);
    });
    return await promise;
  }

  post(item: TEntity, route: string = null, params: {} = {}) {
    return this.http.post<TEntity>(this.getApiUrl(route), item, {
      params: params,
      headers: this.buildHeaders()
    });
  }

  postAny(item: any, route: string = null, params: {} = {}) {
    return this.http.post(this.getApiUrl(route), item, {
      params: params,
      headers: this.buildHeaders()
    });
  }

  async postAsync(item: TEntity, route: string = null, params: {} = {}) {
    const promise = this.http
      .post<TEntity>(this.getApiUrl(route), item, {
        params: params,
        headers: this.buildHeaders()
      })
      .toPromise();
    promise.catch((response: any) => this.handleError(response));
    await promise;
  }

  async postAnyAsync(route: string, item: any, params: {} = {}) {
    const promise = this.http
      .post<TEntity>(this.getApiUrl(route), item, {
        params: params,
        headers: this.buildHeaders()
      })
      .toPromise();
    promise.catch((response: any) => this.handleError(response));
    await promise;
  }

  put(item: TEntity, route: string = null, params: {} = {}) {
    return this.http.put<TEntity>(
      `${this.getApiUrl(route)}/${item['id']}`,
      item,
      {
        params: params,
        headers: this.buildHeaders()
      }
    );
  }

  async putAsync(item: TEntity, route: string = null, params: {} = {}) {
    if (!item['id']) {
      item['id'] = Guid.newGuid();
    }
    const promise = this.http
      .put<TEntity>(`${this.getApiUrl(route)}/${item['id']}`, item, {
        params: params,
        headers: this.buildHeaders()
      })
      .toPromise();
    promise.catch((error: any) => this.handleError(error));
    await promise;
  }

  async putAnyAsync(id: string, route: string, params: {} = {}) {
    const promise = this.http
      .put<TEntity>(this.getApiUrl(route, id), params, {
        headers: this.buildHeaders()
      })
      .toPromise();
    promise.catch((error: any) => this.handleError(error));
    await promise;
  }

  patch(item: TEntity, route: string = null, params: {} = {}) {
    return this.http.patch(this.getApiUrl(route), item, {
      params: params,
      headers: this.buildHeaders()
    });
  }

  patchAny(item: any, route: string = null, params: {} = {}) {
    return this.http.patch(this.getApiUrl(route), item, {
      params: params,
      headers: this.buildHeaders()
    });
  }

  async patchAsync(item: TEntity, route: string = null, params: {} = {}) {
    const promise = this.http
      .patch<TEntity>(this.getApiUrl(route), item, {
        params: params,
        headers: this.buildHeaders()
      })
      .toPromise();
    promise.catch((response: any) => this.handleError(response));
    await promise;
  }

  async patchAnyAsync(route: string, item: any, params: {} = {}) {
    const promise = this.http
      .patch<TEntity>(this.getApiUrl(route), item, {
        params: params,
        headers: this.buildHeaders()
      })
      .toPromise();
    promise.catch((response: any) => this.handleError(response));
    await promise;
  }

  delete(id: string) {
    return this.http.delete<TEntity>(this.getApiUrl(id), {
      headers: this.buildHeaders()
    });
  }

  patchBatch(route: string, ids: string[] = null, items: TEntity[] = null) {
    return this.http.patch<TEntity>(this.getApiUrl(route), ids ?? items, {
      headers: this.buildHeaders()
    });
  }

  async deleteAsync(route: string = null, id: string) {
    const promise = this.http
      .delete<TEntity>(this.getApiUrl(route, id), {
        headers: this.buildHeaders()
      })
      .toPromise();
    promise.catch((error: any) => {
      this.handleError(error);
    });
    await promise;
  }

  getBlobResource(path: string): Observable<any> {
    return this.http.get(`${this.getApiUrl()}/${path}`, {
      responseType: 'blob',
      headers: this.buildHeaders()
    });
  }

  protected getApiUrl(route: string = null, id: string = null): string {
    return `${this.origin}/${this.endpoint}${id ? '/' + id : ''}${
      route ? '/' + route : ''
    }`;
  }

  protected handleError(response: any) {
    if (response.status !== 400) {
      if (
        !(
          response.status === 200 &&
          response.url &&
          response.url.toLowerCase().indexOf('account/login') >= 0
        ) &&
        response.status !== 401 &&
        response.status !== 402
      ) {
        if (!response.error || !response.error.custom) {
          console.log(
            'An error occured during saving the requested information.'
          );
        }
      }
    } else {
      console.log(response.error);
    }
  }

  protected buildHeaders(): HttpHeaders {
    let headers = new HttpHeaders();
    headers.set('Accept', '*/*');
    headers.set('Access-Control-Allow-Credentials', 'true');
    return headers;
  }
}
