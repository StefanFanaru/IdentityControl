import { SortDirection } from '../sortDirection';

export interface IdentityTableQuery {
  pageIndex: number;
  pageSize: number;
  sortColumn: string;
  sortDirection: SortDirection;
  filterType: any;
  searchTerm: string;
  id: string;
  relation: string;
}
