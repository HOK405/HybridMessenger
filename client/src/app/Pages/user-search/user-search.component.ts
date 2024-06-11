import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpService } from '../../services/http.service';

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
  pageSize: number = 3;
  hasMoreData: boolean = true;

  constructor(private fb: FormBuilder, private httpService: HttpService) {
    this.searchForm = this.fb.group({
      sortBy: ['CreatedAt', [Validators.required]],
      searchValue: [''],
      ascending: [true],
      fields: [''],
    });
  }

  ngOnInit(): void {
    this.handleSearch(); // Load users on component initialization
  }

  handleSearch(page: number = 1): void {
    if (this.searchForm.invalid) {
      return;
    }

    const searchModel: PaginationRequest = {
      ...this.searchForm.value,
      pageNumber: page,
      pageSize: this.pageSize,
      fields: this.searchForm.value.fields
        ? this.searchForm.value.fields
            .split(',')
            .map((field: string) => field.trim())
        : [],
    };

    this.httpService.post<any[]>('User/get-paged', searchModel).subscribe(
      (response) => {
        this.users = response;
        this.hasMoreData = response.length === this.pageSize;
        this.currentPage = page;
        this.searchResult = 'Users retrieved successfully!';
        this.userRequestedFields = searchModel.fields.length
          ? searchModel.fields
          : this.allUserDtoFields;
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
