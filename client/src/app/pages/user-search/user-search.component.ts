import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../../services/user.service';

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
  allUserFields: string[] = [
    'Id',
    'UserName',
    'Email',
    'CreatedAt',
    'PhoneNumber',
  ];
  currentPage: number = 1;
  pageSize: number = 10;
  hasMoreData: boolean = true;

  dropdownList: string[] = this.allUserFields;
  selectedItems: string[] = this.allUserFields;
  dropdownSettings = {};

  constructor(private fb: FormBuilder, private userService: UserService) {
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

  private constructSearchModel(page: number): any {
    return {
      ...this.searchForm.value,
      pageNumber: page,
      pageSize: this.pageSize,
      fields: this.selectedItems,
    };
  }

  private handleSearchResponse(response: any[], page: number): void {
    this.users = response;
    this.hasMoreData = response.length === this.pageSize;
    this.currentPage = page;
    this.userRequestedFields = this.selectedItems.length ? this.selectedItems : this.allUserFields;
    this.searchResult = null;
  }

  private handleSearchError(error: any): void {
    this.searchResult = `Search failed: ${error.message}`;
    console.error('Error:', error);
  }

  handleSearch(page: number = 1): void {
    if (this.searchForm.invalid) {
      return;
    }

    const searchModel = this.constructSearchModel(page);

    this.userService.searchUsers(searchModel).subscribe(
      (response) => this.handleSearchResponse(response, page),
      (error) => this.handleSearchError(error)
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
