export interface PaginationRequest {
    pageNumber: number;
    pageSize: number;
    sortBy: string;
    searchValue: string;
    ascending: boolean;
    fields: string[];
  }