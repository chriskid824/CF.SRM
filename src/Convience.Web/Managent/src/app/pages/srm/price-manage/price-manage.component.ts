import { Component, OnInit, TemplateRef } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { LayoutComponent } from '../../layout/layout/layout.component';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { SrmRfqService } from '../../../business/srm/srm-rfq.service';
import { StorageService } from '../../../services/storage.service';
import { SrmPriceService } from '../../../business/srm/srm-price.service';

@Component({
  selector: 'app-price-manage',
  templateUrl: './price-manage.component.html',
  styleUrls: ['./price-manage.component.less']
})
export class PriceManageComponent implements OnInit {
  page: number = 1;
  size: number = 6;
  total: number;
  data;
  MatnrList;
  VendorList;
  selectedMatnrId;
  selectedVendorId;
  tplModal: NzModalRef;
  searchForm: FormGroup = new FormGroup({});
  matnrListForm: FormGroup = new FormGroup({});
  vendorListForm: FormGroup = new FormGroup({});



  constructor(private _formBuilder: FormBuilder
    , private _router: Router
    , private _layout: LayoutComponent
    , private _storageService: StorageService
    , private _srmPriceService: SrmPriceService
    , private _modalService: NzModalService
    , private _srmRfqService: SrmRfqService,
  ) { }

  ngOnInit(): void {
    this.searchForm = this._formBuilder.group({
      matnr: [null],
      vendor: [null],
      status: [1],
      queryMatnr: [null],
      queryVendor:[null]
    });
  }

  pageChange() {
    this.refresh();
  }
  sizeChange() {
    this.page = 1;
    this.refresh();
  }
  submitSearch() {
    this.page = 1;
    this.refresh();
  }
  refresh() {
    if (!this.selectedMatnrId && !this.selectedVendorId) { alert('料號或供應商擇一必填!'); return;}
    var query = {
      matnrId: this.selectedMatnrId,
      vendorId: this.selectedVendorId,
      status: this.searchForm.value["status"] == null ? "" : this.searchForm.value["status"],
      page: this.page,
      size: this.size
    }
    this._srmPriceService.QueryInfoRecord(query).subscribe(result => {
      this.data = result["data"];
      this.total = result["count"];
      console.log(result);
      console.log(this.data);
    })
  }

  add(title: TemplateRef<{}>, content: TemplateRef<{}>) {
    this.tplModal = this._modalService.create({
      nzTitle: title,
      nzContent: content,
      nzFooter: null,
      nzClosable: true,
      nzMaskClosable: false,
      nzWidth: 1000,
    });
  }
  cancelEdit() {
    this.tplModal.close();
  }
  //refresh
  searchMatnrList() {
    this.onRefreshMatnr();
  }
  searchVendorList() {
    this.onRefreshVendor();
  }
  onRefreshMatnr() {
    var matnrQuery = {
      matnr: this.searchForm.get("queryMatnr")?.value,
      page: 1,
      size: 50
    }
    this._srmRfqService.GetMatnr(matnrQuery).subscribe(result => {
      this.MatnrList = [];
      console.log(result);
      //this.OriMatnrList = result["data"];
      //for (var i = 0; i < this.OriMatnrList.length; i++) {
      //  this.MatnrList.push({ mantr: this.OriMatnrList[i].matnr, label: this.OriMatnrList[i].matnr + ' ' + this.OriMatnrList[i].description?.substring(0, 40) , value: this.OriMatnrList[i].matnrId });
      //}
      this.MatnrList = result["data"];
      if (result["count"] > matnrQuery.size) {
        alert('查詢結果筆數：' + result['count'] + '(系統只顯示最前 ' + matnrQuery.size + ' 筆資料) ，請重新指定查詢條件!');
      }
    });
  }

  onRefreshVendor() {
    var vendorQuery = {
      vendor: this.searchForm.value["queryVendor"],
      page: 1,
      size: 50
    }
    this._srmRfqService.GetVendor(vendorQuery).subscribe(result => {
      this.VendorList = [];
      //this.OriVendorList = result["data"];
      //for (var i = 0; i < this.OriVendorList.length; i++) {
      //  this.VendorList.push({ vendor: this.OriVendorList[i].vendor,  label: this.OriVendorList[i].vendor + ' ' + this.OriVendorList[i].vendorName?.substring(0, 40), value: this.OriVendorList[i].vendorId });
      //}
      this.VendorList = result["data"];
      if (result["count"] > vendorQuery.size) {
        alert('查詢結果筆數：' + result['count'] + '(系統只顯示最前 ' + vendorQuery.size + ' 筆資料) ，請重新指定查詢條件!');
      }
    });
  }

  selectedMatnr(data) {
    this.searchForm.patchValue({
      matnr: data.srmMatnr1
    });
    this.selectedMatnrId = data.matnrId;
    console.log(data.matnrId);
    this.tplModal.close();
  }

  selectedVendor(data) {
    this.searchForm.patchValue({
      vendor: data.srmVendor1
    });
    this.selectedVendorId = data.vendorId;
    console.log(data.vendorId);
    this.tplModal.close();
  }
  reset() {
    this.searchForm.patchValue({
      matnr:"",
      vendor:""
    });
    this.selectedMatnrId = "";
    this.selectedVendorId = "";
  }
  openInfoRecord(rfqId,caseId) {
    this._layout.navigateTo('price');
    this._router.navigate(['srm/price', { id: rfqId, caseId: caseId }]);
  }
}
