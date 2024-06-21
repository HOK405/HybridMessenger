import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../../services/user.service';

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

  constructor(private userService: UserService, private fb: FormBuilder) {
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

  private constructQuery(page: number): any {
    return {
      pageNumber: page,
      pageSize: this.pageSize,
      sortBy: this.messageForm.value.sortBy,
      searchValue: this.messageForm.value.searchValue,
      ascending: this.messageForm.value.ascending,
      fields: this.selectedItems,
    };
  }

  private handleResponse(response: any[], page: number): void {
    this.messages = response;
    this.hasMoreData = response.length === this.pageSize;
    this.currentPage = page;
  }

  private handleError(error: any): void {
    this.messageResult = `Failed to retrieve messages: ${error.message}`;
    console.error('Error:', error);
  }

  loadMessages(page: number = 1): void {
    if (this.messageForm.invalid) {
      return;
    }

    const query = this.constructQuery(page);
    this.displayedFields = query.fields.length ? query.fields : this.allMessageFields;

    this.userService.getUserMessages(query).subscribe(
      (response) => this.handleResponse(response, page),
      (error) => this.handleError(error)
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
