import { Component, OnInit,ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { SrmRfqService } from '../../../business/srm/srm-rfq.service';
import { Rfq } from '../model/rfq';
import { StorageService } from '../../../services/storage.service';
import { Router } from '@angular/router';
import { LayoutComponent } from '../../layout/layout/layout.component';
@Component({
  selector: 'app-rfq-manage',
  templateUrl: './rfq-manage.component.html',
  styleUrls: ['./rfq-manage.component.less']
})
export class RfqManageComponent implements OnInit {
  searchForm: FormGroup = new FormGroup({});
  data;
  status;

  page: number = 1;
  size: number = 6;
  total: number;
  t = "";

  _rfqNum;
  _matnr;
  _status;
  _name;

  constructor(private _formBuilder: FormBuilder,
    private _srmRfqService: SrmRfqService,
    private _storageService: StorageService,
    private _router: Router,
    private _layout: LayoutComponent  ) { }


  selectedValue = null

  ngOnInit(): void {
    this.searchForm = this._formBuilder.group({
      RFQ_NUM: [null],
      STATUS: ["1"],
      NAME: [null],
      MATNR:[null],
    });
    if (sessionStorage.getItem("rfq-manage")) {
      var query = JSON.parse(sessionStorage.getItem("rfq-manage"));
      this._rfqNum = query.rfqNum;
      this._matnr = query.matnr;
      this._name = query.name;
      this._status = query.status;
      this.page = query.page;
      this.size = query.size;
      console.log(this._status);
      this.searchForm.patchValue({
        RFQ_NUM: this._rfqNum,
        MATNR: this._matnr,
        NAME: this._name,
        STATUS: this._status
      });
      console.log(this.searchForm);
      this.refresh();
    }
  }

  submitSearch() {
    this._rfqNum = this.searchForm.value["RFQ_NUM"] == null ? "" : this.searchForm.value["RFQ_NUM"];
    this._matnr = this.searchForm.value["MATNR"] == null ? "" : this.searchForm.value["MATNR"];
    this._status = this.searchForm.value["STATUS"] == null ? "" : this.searchForm.value["STATUS"];
    this._name = this.searchForm.value["NAME"] == null ? "" : this.searchForm.value["NAME"];
    this.page = 1;
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
    //this.t = this.test();
    console.log(this.searchForm.value["STATUS"]);
    var query = {
      rfqNum: this._rfqNum,
      matnr: this._matnr,
      status: this._status,
      name: this._name,
      page: this.page,
      size: this.size,
      orderDesc:true,
    }
    console.log(query);
    this._srmRfqService.GetRfqList(query).subscribe(result => {
      this.data = result["data"];
      this.total = result["count"];
      console.log(this.data);
    })
    this.status = query.status;
    sessionStorage.setItem("rfq-manage", JSON.stringify(query));
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

  sizeChange() {
    this.page = 1;
    this.refresh();
  }

  open(id) {
    this._layout.navigateTo('rfq');
    this._router.navigate(['srm/rfq', { id: id }]);
    //window.open('../srm/rfq?id=' + id);
  }
  openPrice(id) {
    this._layout.navigateTo('price');
    this._router.navigate(['srm/price', { id: id  }]);
    //window.open('../srm/rfq?id=' + id);
  }
  addRfq() {
    this._layout.navigateTo('rfq');
    this._router.navigate(['srm/rfq']);
    //window.open('../srm/rfq');
  }
}
