import {SortDirection} from './sortDirection';

export interface Pagination {
  pageIndex: number;
  pageSize: number;
  sortColumn: string;
  sortDirection: SortDirection;
  filterType: any;
  searchTerm: string;
}
