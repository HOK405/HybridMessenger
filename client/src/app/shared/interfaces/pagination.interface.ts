export interface Pagination {
    pageNumber: number;
    pageSize: number;
    sortBy: string;
    searchValue: string;
    ascending: boolean;
    fields: string[];
  }