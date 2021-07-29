import { Component, OnInit, ViewEncapsulation, ViewChild, TemplateRef } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, FormControl} from '@angular/forms';
import * as $ from 'jquery';
import { of } from 'rxjs';
import 'ag-grid-community/dist/styles/ag-theme-alpine.css';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { SrmRfqService } from '../../../business/srm/srm-rfq.service';
import { DatePipe } from '@angular/common'
import { StorageService } from '../../../services/storage.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-rfq',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './rfq.component.html',
  styleUrls: ['./rfq.component.less'],
})
export class RfqComponent implements OnInit {
  H;
  canModify;
  canCxl;
  canDel;
  name;
  page = 1;
  size = 50;

  //Matnr
  columnDefs;
  rowData_MATNR;
  columnApi;
  gridApi;
  defaultColDef;
  OriMatnrList;
  MatnrList;
  //Vendor
  columnDefs_VENDOR;
  rowData_VENDOR;
  columnApi_VENDOR;
  gridApi_VENDOR;
  defaultColDef_VENDOR;
  OriVendorList;
  VendorList;
  //sorcer
  SourcerList;
  radioValue;

  rowSelection = "multiple";
  tplModal: NzModalRef;
  searchForm: FormGroup = new FormGroup({});
  formDetail: FormGroup = new FormGroup({});
  matnrListForm: FormGroup = new FormGroup({});
  vendorListForm: FormGroup = new FormGroup({});
  sourcerListForm: FormGroup = new FormGroup({});


  constructor(
    private activatedRoute: ActivatedRoute,
    private _storageService: StorageService,
    private formBuilder: FormBuilder,
    private _modalService: NzModalService,
    private _formBuilder: FormBuilder,
    private _srmRfqService: SrmRfqService,
    public datepipe: DatePipe) {
    console.log(_storageService.costNo);
    this.H = {
    }
    this.activatedRoute.queryParams.subscribe(params => {
      this.H.rfqId = params['id'];
      console.log("RFQID:" + this.H.rfqId); // Print the parameter to the console.
    });


    this.defaultColDef = {
      filter: "agTextColumnFilter",
      allowedAggFuncs: ['sum', 'min', 'max'],
      enableValue: true,
      enableRowGroup: true,
      enablePivot: true,
      resizable: true,
      rowSelection: "multiple",
      wrapText: true,
    };

    this.columnDefs_VENDOR = [
      {
        headerName: "供應商代碼",
        field: "vendor",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "250px",
        cellRenderer: 'agGroupCellRenderer',
        headerCheckboxSelection: true,
        checkboxSelection: true,
      },
      {
        headerName: "供應商名稱",
        field: "vendorName",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "聯絡人",
        field: "person",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "地址",
        field: "address",
        enableRowGroup: true,
        cellClass: "show-cell",
        autoHeight: true,
        wrapText: true,
      },
      {
        headerName: "電話號碼",
        field: "telPhone",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "分機",
        field: "ext",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "傳真號碼",
        field: "faxNumber",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "手機號碼",
        field: "cellPhone",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "信箱",
        field: "mail",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "狀態",
        field: "status",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      }
    ]
    this.rowData_MATNR = [];
    this.rowData_VENDOR = [];
  }

  onRefreshMatnr() {
    var matnrQuery = {
      matnr: this.searchForm.get("matnr")?.value,
      page: this.page,
      size: this.size
    }
    this._srmRfqService.GetMatnr(matnrQuery).subscribe(result => {
      this.MatnrList = [];
      console.log(result);
      this.OriMatnrList = result["data"];
      for (var i = 0; i < this.OriMatnrList.length; i++) {
        this.MatnrList.push({ mantr: this.OriMatnrList[i].matnr, label: this.OriMatnrList[i].matnr + ' ' + this.OriMatnrList[i].description?.substring(0, 40) , value: this.OriMatnrList[i].matnrId });
      }
      if (result["count"] > matnrQuery.size) {
        alert('查詢結果筆數：' + result['count'] + '(系統只顯示最前 ' + matnrQuery.size + ' 筆資料) ，請重新指定查詢條件!');
      }
    });
  }

  onRefreshVendor() {
    var vendorQuery = {
      vendor: this.searchForm.value["vendor"],
      page: this.page,
      size: this.size
    }
    this._srmRfqService.GetVendor(vendorQuery).subscribe(result => {
      this.VendorList = [];
      this.OriVendorList = result["data"];
      for (var i = 0; i < this.OriVendorList.length; i++) {
        this.VendorList.push({ vendor: this.OriVendorList[i].vendor,  label: this.OriVendorList[i].vendor + ' ' + this.OriVendorList[i].vendorName?.substring(0, 40), value: this.OriVendorList[i].vendorId });
      }
      if (result["count"] > vendorQuery.size) {
        alert('查詢結果筆數：' + result['count'] + '(系統只顯示最前 ' + vendorQuery.size + ' 筆資料) ，請重新指定查詢條件!');
      }
    });
  }
  onRefreshSourcer() {
    var query = {
      name: (this.searchForm.value["name"])??"",
      costNo: this._storageService.costNo,
      page: this.page,
      size: this.size
    }

    this._srmRfqService.GetSourcerList(query)
      .subscribe(result => {
        console.log(result['data']);
        this.SourcerList = [];
        console.log(result['data'].length);
        for (var i = 0; i < result['data'].length; i++) {
          console.log(this.SourcerList);
          this.SourcerList.push({ label: result['data'][i].name, value: result['data'][i] });
        }
        if (result['count'] > query.size) {
          alert('查詢結果筆數：' + result['count'] + '(系統只顯示最前 ' + query.size + ' 筆資料) ，請重新指定查詢條件!');
        }
      });
  }
  onGridReady(params) {
    function myRowClickedHandler(event) {
      console.log('The row was clicked');
    }
    params.api.addEventListener('rowClicked', myRowClickedHandler);
    this.gridApi = params.api;
    this.columnApi = params.columnApi;
    this.gridApi.sizeColumnsToFit();
  }


  onGridReady_VENDOR(params) {
    function myRowClickedHandler(event) {
      console.log('The row was clicked');
    }
    params.api.addEventListener('rowClicked', myRowClickedHandler);
    this.gridApi_VENDOR = params.api;
    this.columnApi_VENDOR = params.columnApi;
    this.gridApi_VENDOR.sizeColumnsToFit();
  }

  ngOnInit(): void {
    console.log('test');
    if (this.H.rfqId != undefined) {
      this.name = this.H.CREATEBY;
      console.log('GetRfqData');
      this._srmRfqService.GetRfqData(this.H.rfqId).subscribe(result => {
        console.log(result);
        console.log(result["h"]);
        this.H = result["h"];
        var c = new Date(this.H.createDate);
        this.H.C_Date = c.getFullYear() + '-' + (c.getMonth() + 1) + '-' + c.getDate();
        this.rowData_MATNR = result["m"];
        this.rowData_VENDOR = result["v"];
        this.canModify = this.H.status == 1;
        this.canCxl = this.H.status == 7;
        this.canDel = this.H.status == 1;
        this.initMatnrList();
        this.formDetail.setValue({ sourcerName: this.H.sourcerName, sourcer: this.H.sourcer, deadline: this.H.deadline });
      });
    } else {
      this.name = this._storageService.Name;
      this.H.CREATEBY = this._storageService.userName;
      this.H.STATUS = 1;
      this.canModify = true;
      this.H.viewstatus = "初始";
    }
    this.initMatnrList();
    this.H.LASTUPDATEBY = this._storageService.userName;
    console.log(this.name);
    $($('.listbox')[1]).hide();
    //this.onRefreshMatnr();
    this.searchForm = this._formBuilder.group({
      matnr: [null],
      vendor: [null],
      sourcer: [null]
    });
    this.formDetail = this._formBuilder.group({
      sourcer: [null],
      sourcerName: [null],
      deadline: [null]
    });
  }


  initMatnrList() {
    this.columnDefs = [
      {
        headerName: "料號",
        field: "matnr",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
        cellRenderer: 'agGroupCellRenderer',
        headerCheckboxSelection: true,
        checkboxSelection: true,
      },
      {
        headerName: "版次",
        field: "version",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "材質規格",
        field: "material",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "體積",
        field: "volume",
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "密度",
        field: "density",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "重量",
        field: "weight",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "狀態",
        field: "status",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "數量",
        field: "qty",
        enableRowGroup: true,
        cellClass: "show-cell",
        editable: this.canModify,
        width: "150px",
      },
      {
        headerName: "機種",
        field: "machineName",
        enableRowGroup: true,
        cellClass: "show-cell",
        editable: this.canModify,
        width: "150px",
      },
      {
        headerName: "備註",
        field: "note",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      }
    ]
  }

  add(title: TemplateRef<{}>, content: TemplateRef<{}>) {
    this.tplModal = this._modalService.create({
      nzTitle: title,
      nzContent: content,
      nzFooter: null,
      nzClosable: true,
      nzMaskClosable: false,
      nzWidth:1000
    });
  }

  deleteMatnr() {
    let selectedNodes = this.gridApi.getSelectedNodes();
    let selectedData = selectedNodes.map(node => node.data);
    let selectedRows = this.gridApi.getSelectedRows();
    let temp = this.rowData_MATNR;
    for (var i = selectedRows.length - 1; i >= 0; i--) {
      for (var j = 0; j < this.rowData_MATNR.length; j++) {
        if (this.rowData_MATNR[j].matnr == selectedRows[i].matnr) {
          temp.splice(j, 1);
          break;
        }
      }
    }
    this.rowData_MATNR = temp;
    this.gridApi.setRowData(this.rowData_MATNR);
    console.log(selectedRows);
    //this.gridApi.applyTransaction({ remove: selectedRows });
  }

  deleteVendor() {
    let selectedNodes = this.gridApi_VENDOR.getSelectedNodes();
    let selectedData = selectedNodes.map(node => node.data);
    let selectedRows = this.gridApi_VENDOR.getSelectedRows();
    let temp = this.rowData_VENDOR;
    for (var i = selectedRows.length - 1; i >= 0; i--) {
      for (var j = 0; j < this.rowData_VENDOR.length; j++) {
        if (this.rowData_VENDOR[j].vendor == selectedRows[i].vendor) {
          temp.splice(j, 1);
          break;
        }
      }
    }
    this.rowData_VENDOR = temp;
    this.gridApi_VENDOR.setRowData(this.rowData_VENDOR);
    console.log(selectedRows);
    //this.gridApi.applyTransaction({ remove: selectedRows });
  }
  //選取
  submitMatnrList() {
    console.log(this.rowData_MATNR);
    for (var i = 0; i < this.MatnrList.length; i++) {
      if (this.MatnrList[i].checked == true) {
        console.log(this.MatnrList[i].label);
        if (this.rowData_MATNR.filter(item => item.matnr.toUpperCase().indexOf(this.MatnrList[i].mantr.toUpperCase()) >= 0).length == 0) {
          this.rowData_MATNR.push(this.OriMatnrList.filter(item => item.matnr == this.MatnrList[i].mantr)[0]);
        }
      }
    }
    this.rowData_MATNR.forEach(row => row.volume = row.length + 'X' + row.width + 'X' + row.height);
    this.gridApi.setRowData(this.rowData_MATNR);
    console.log(this.rowData_MATNR);
    this.tplModal.close();
  }

  submitVendorList() {
    console.log(this.rowData_VENDOR);
    for (var i = 0; i < this.VendorList.length; i++) {
      if (this.VendorList[i].checked == true) {
        console.log(this.VendorList[i].label);
        if (this.rowData_VENDOR.filter(item => item.vendor.toUpperCase().indexOf(this.VendorList[i].vendor.toUpperCase()) >= 0).length == 0) {
          this.rowData_VENDOR.push(this.OriVendorList.filter(item => item.vendor == this.VendorList[i].vendor)[0]);
        }
      }
    }
    this.gridApi_VENDOR.setRowData(this.rowData_VENDOR);
    console.log(this.rowData_VENDOR);
    this.tplModal.close();
  }

  submitSourcerList() {
    console.log(this.SourcerList);
    console.log(this.radioValue);
    this.formDetail.setValue({ sourcerName: this.radioValue.name, sourcer: this.radioValue.userName, deadline: this.formDetail.value["deadline"] });
    this.tplModal.close();
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
  searchSourcerList() {
    this.onRefreshSourcer();
  }

  getrfq() {
    var rfq = {
      h: null, m: null, v: null
    }
    var date = new Date();
    this.H.SOURCER = this.formDetail.value["sourcer"];

    rfq.h = this.H;
    console.log('dead' + this.formDetail.get('deadline').value);
    if (this.formDetail.value["deadline"] != null) {
      var deadline = new Date(this.formDetail.value["deadline"]);
      rfq.h.deadLine = deadline.getFullYear() + '-' + (deadline.getMonth() + 1) + '-' + deadline.getDate();
    } else {
      rfq.h.deadLine = null;
    }
    rfq.h.LASTUPDATEBY = this._storageService.userName;
    console.log(this.gridApi);
    this.gridApi.forEachNode(node => console.log(node));
    rfq.m = [];
    rfq.v = [];
    //this.rowData_MATNR.forEach(row => rfq.m.push(row));
    //this.rowData_VENDOR.forEach(row => rfq.v.push(row));
    this.gridApi.forEachNode(node => rfq.m.push(node.data));
    this.gridApi_VENDOR.forEachNode(node => rfq.v.push(node.data));
    return rfq;
  }

  save() {
    var rfq = this.getrfq();
    this._srmRfqService.SAVE(rfq).subscribe(result => {
      console.log(result);
      alert('保存成功');
      window.close();
    });
  }

  start() {
    var rfq = this.getrfq();
    if (rfq.h.SOURCER == null) { alert("詢價人員未選擇"); return; }
    if (rfq.h.deadLine == null) { alert("截止日期未選擇"); return; }
    if (rfq.m.length == 0) { alert('料號至少需一筆!'); return; }
    for (var i = 0; i < rfq.m.length; i++) {
      if (!rfq.m[i].qty) {
        alert("料號" + rfq.m[i].matnr + "數量未填");
        return;
      }
      if (isNaN(rfq.m[i].qty)) {
        alert("料號" + rfq.m[i].matnr + "數量格式錯誤");
        return;
      }
      if (!rfq.m[i].machineName) {
        alert("料號" + rfq.m[i].matnr + "機碼未填");
        return;
      }
    }
    if (rfq.v.length == 0) { alert('供應商至少需一筆!'); return; }
    this._srmRfqService.StartUp(rfq).subscribe(result => {
      alert('上架成功');
      window.close();
    });
  }

  cancel() {
    var rfq = this.getrfq();
    console.log(rfq);
    rfq.h.endBy = this._storageService.userName;
    this._srmRfqService.Cancel(rfq.h).subscribe(result => {
      alert('作廢成功');
      window.close();
    });
  }

  delete() {
    var rfq = this.getrfq();
    rfq.h.endBy = this._storageService.userName;
    this._srmRfqService.Delete(rfq.h).subscribe(result => {
      alert('刪除成功');
      window.close();
    });
  }
}
