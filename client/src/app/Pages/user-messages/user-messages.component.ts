import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
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
  selector: 'app-user-messages',
  templateUrl: './user-messages.component.html',
  styleUrls: ['./user-messages.component.css'],
})
export class UserMessagesComponent implements OnInit {
  messages: any[] = [];
  currentPage: number = 1;
  pageSize: number = 10;
  hasMoreData: boolean = true;
  private baseUrl = environment.baseUrl;
  messageResult: string | null = null;

  messageForm: FormGroup;

  allMessageFields: string[] = [
    'MessageId',
    'ChatId',
    'UserId',
    'MessageText',
    'SentAt',
    'SenderUserName',
  ];
  displayedFields: string[] = this.allMessageFields;

  dropdownList: string[] = this.allMessageFields;
  selectedItems: string[] = this.allMessageFields;
  dropdownSettings = {};

  constructor(private http: HttpClient, private fb: FormBuilder) {
    this.messageForm = this.fb.group({
      sortBy: ['SentAt', Validators.required],
      searchValue: [''],
      fields: [this.selectedItems],
      ascending: [true],
    });
  }

  ngOnInit(): void {
    this.dropdownSettings = {
      singleSelection: false,
      idField: 'item',
      textField: 'item',
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      allowSearchFilter: true,
    };
    this.loadMessages();
  }

  loadMessages(page: number = 1): void {
    if (this.messageForm.invalid) {
      return;
    }

    const query: PaginationRequest = {
      pageNumber: page,
      pageSize: this.pageSize,
      sortBy: this.messageForm.value.sortBy,
      searchValue: this.messageForm.value.searchValue,
      ascending: this.messageForm.value.ascending,
      fields: this.selectedItems,
    };

    this.displayedFields = query.fields.length
      ? query.fields
      : this.allMessageFields;

    const token = localStorage.getItem('accessToken');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });

    this.http
      .post<any[]>(`${this.baseUrl}Message/get-user-messages`, query, {
        headers,
      })
      .subscribe(
        (response) => {
          this.messages = response;
          this.hasMoreData = response.length === this.pageSize;
          this.currentPage = page;
        },
        (error) => {
          this.messageResult = `Failed to retrieve messages: ${error.message}`;
          console.error('Error:', error);
        }
      );
  }

  nextPage(): void {
    if (this.hasMoreData) {
      this.loadMessages(this.currentPage + 1);
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.loadMessages(this.currentPage - 1);
    }
  }
}
