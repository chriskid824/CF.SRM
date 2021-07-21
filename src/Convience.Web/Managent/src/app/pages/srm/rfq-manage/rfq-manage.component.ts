import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { RFQ } from '../model/rfq';


@Component({
  selector: 'app-rfq-manage',
  templateUrl: './rfq-manage.component.html',
  styleUrls: ['./rfq-manage.component.less']
})
export class RfqManageComponent implements OnInit {
  searchForm: FormGroup = new FormGroup({});
  data: RFQ[] = [];

  page: number = 1;
  size: number = 10;
  total: number = 0;
  t = "";
  constructor(private _formBuilder: FormBuilder,) { }


  selectedValue = null;
  countries = [
    { id: 1, name: "United States" },
    { id: 2, name: "Australia" },
    { id: 3, name: "Canada" },
    { id: 4, name: "Brazil" },
    { id: 5, name: "England" }
  ];

  ngOnInit(): void {
  }

  submitSearch() {

    this.refresh();
  }

  test(): string {
    var d = new Date(),
      month = '' + (d.getMonth() + 1),
      day = '' + d.getDate(),
      year = d.getFullYear();

    if (month.length < 2)
      month = '0' + month;
    if (day.length < 2)
      day = '0' + day;

    return [year, month, day].join('-');
  };

  refresh() {
    this.t = this.test();

    this.data = [
      { RFQ_ID: 1, RFQ_NUM: 'order 1', STATUS: 1, SOURCER: 'TEST', DateString: this.t,CREATE_BY:'TEST'},
      { RFQ_ID: 2, RFQ_NUM: 'order 2', STATUS: 1, SOURCER: 'TEST', DateString: this.t,CREATE_BY:'TEST' },
      { RFQ_ID: 3, RFQ_NUM: 'order 3', STATUS: 1, SOURCER: 'TEST', DateString: this.t,CREATE_BY:'TEST' },
      { RFQ_ID: 4, RFQ_NUM: 'order 4', STATUS: 1, SOURCER: 'TEST', DateString: this.t,CREATE_BY:'TEST' }
    ];
  }

  // reset the search form content 
  resetSearchForm() {
    this.searchForm = this._formBuilder.group({
      userName: [null],
      phoneNumber: [null],
      name: [null],
      roleid: [null],
      position: [null]
    });
  }


  pageChange() {
    this.refresh();
  }
}
