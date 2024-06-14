import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

interface PaginationRequest {
  pageNumber: number;
  pageSize: number;
  sortBy: string;
  searchValue: string;
  ascending: boolean;
  fields: string[];
}

@Component({
  selector: 'app-user-search',
  templateUrl: './user-search.component.html',
  styleUrls: ['./user-search.component.css'],
})
export class UserSearchComponent implements OnInit {
  searchForm: FormGroup;
  users: any[] = [];
  userRequestedFields: string[] = [];
  searchResult: string | null = null;
  allUserDtoFields: string[] = [
    'Id',
    'UserName',
    'Email',
    'CreatedAt',
    'PhoneNumber',
  ];
  currentPage: number = 1;
  pageSize: number = 10;
  hasMoreData: boolean = true;
  private baseUrl = environment.baseUrl;

  dropdownList: string[] = this.allUserDtoFields;
  selectedItems: string[] = this.allUserDtoFields;
  dropdownSettings = {};

  constructor(private fb: FormBuilder, private http: HttpClient) {
    this.searchForm = this.fb.group({
      sortBy: ['CreatedAt', [Validators.required]],
      searchValue: [''],
      ascending: [true],
      fields: [this.selectedItems],
    });
  }

  ngOnInit(): void {
    this.dropdownSettings = {
      singleSelection: false,
      idField: 'item',
      textField: 'item',
      selectAllText: 'Select all',
      unSelectAllText: 'Unselect all',
      allowSearchFilter: true,
    };
    this.handleSearch();
  }

  handleSearch(page: number = 1): void {
    if (this.searchForm.invalid) {
      return;
    }

    const searchModel: PaginationRequest = {
      ...this.searchForm.value,
      pageNumber: page,
      pageSize: this.pageSize,
      fields: this.selectedItems,
    };

    this.http
      .post<any[]>(`${this.baseUrl}User/get-paged`, searchModel)
      .subscribe(
        (response) => {
          this.users = response;
          this.hasMoreData = response.length === this.pageSize;
          this.currentPage = page;
          this.userRequestedFields = searchModel.fields.length
            ? searchModel.fields
            : this.allUserDtoFields;
          this.searchResult = null;
        },
        (error) => {
          this.searchResult = `Search failed: ${error.message}`;
          console.error('Error:', error);
        }
      );
  }

  nextPage(): void {
    if (this.hasMoreData) {
      this.handleSearch(this.currentPage + 1);
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.handleSearch(this.currentPage - 1);
    }
  }
}
