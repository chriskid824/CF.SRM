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
  name;
  columnDefs;
  rowData_MATNR;
  columnApi;
  gridApi;
  defaultColDef;
  searchForm: FormGroup = new FormGroup({});
  private _searchObject: any = {};
  columnDefs_VENDOR;
  rowData_VENDOR;
  columnApi_VENDOR;
  gridApi_VENDOR;
  defaultColDef_VENDOR;
  rowSelection = "multiple";

  tplModal: NzModalRef;
  matnrListForm: FormGroup = new FormGroup({});
  vendorListForm: FormGroup = new FormGroup({});
  createForm: FormGroup = new FormGroup({});

  form: FormGroup;
  ordersData = [];
  getOrders() {
    return [
      { id: 100, name: 'order 1' },
      { id: 200, name: 'order 2' },
      { id: 300, name: 'order 3' },
      { id: 400, name: 'order 4' }
    ];
  }

  OriMatnrList;
  MatnrList;
  OriVendorList;
  VendorList;

  selectedValue = null;
  countries = [
    { id: 1, name: "United States" },
    { id: 2, name: "Australia" },
    { id: 3, name: "Canada" },
    { id: 4, name: "Brazil" },
    { id: 5, name: "England" }
  ];

  get ordersFormArray() {
    return this.form.controls.orders as FormArray;
  }
  private addCheckboxes() {
    this.ordersData.forEach(() => this.ordersFormArray.push(new FormControl(false)));
  }
  constructor(private activatedRoute: ActivatedRoute, private _storageService: StorageService, private formBuilder: FormBuilder, private _modalService: NzModalService, private _formBuilder: FormBuilder, private _srmRfqService: SrmRfqService, public datepipe: DatePipe) {
    this.H = {
      status: 0,
      CREATEBY: _storageService.userName,
    }
    this.activatedRoute.queryParams.subscribe(params => {
      this.H.RFQID = params['id'];
      console.log(this.H.RFQID); // Print the parameter to the console.
    });


    //_srmRfqService.GetMatnr().subscribe(result => {
    //  console.log(result);
    //  this.OriMatnrList = result;
    //});
    //_srmRfqService.GetVendor().subscribe(result => {
    //  console.log(result);
    //  this.OriVendorList = result;
    //});

    this.form = this.formBuilder.group({
      orders: new FormArray([])
    });
    of(this.getOrders()).subscribe(orders => {
      //this.ordersData = orders;
      //this.addCheckboxes();
    });
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
        field: "qot",
        enableRowGroup: true,
        cellClass: "show-cell",
        editable: true,
        width: "150px",
      },
      {
        headerName: "機種",
        field: "machineName",
        enableRowGroup: true,
        cellClass: "show-cell",
        editable: true,
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

    this.defaultColDef = {
      filter: "agTextColumnFilter",
      allowedAggFuncs: ['sum', 'min', 'max'],
      enableValue: true,
      enableRowGroup: true,
      enablePivot: true,
      // floatingFilter: true,
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
        field: "telphone",
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
        field: "faxnumber",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "手機號碼",
        field: "cellphone",
        enableRowGroup: true,
        cellClass: "show-cell",
        editable: true,
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


    //this.columnDefs = [
    //  {
    //    headerName: 'Make', field: 'make'},
    //  { headerName: 'Model', field: 'model' },
    //  { headerName: 'Price', field: 'price' }
    //];

    this.rowData_MATNR = [];
    this.rowData_VENDOR = [];

    //this.rowData_MATNR = [
    //  { matnr: 'toyota', version: '1', material: 'celica', volume: '35000', density: '10', weight: '12', status: '1', machine_name: '', note: '1' },
    //  { matnr: 'ford', version: '1', material: 'mondeo', volume: '32000', density: '10', weight: '12', status: '1', machine_name: '', note: '2' },
    //  { matnr: 'porsche', version: '1', material: 'boxter', volume: '72000', density: '10', weight: '12', status: '1', machine_name: '', note: '3' }
    //  , { matnr: 'qq' }
    //];
    //this.rowData_VENDOR = [
    //  { vendor: 'toyota', vendor_name: '1', person: 'celica', address: '35000testwidthtestwidthtestwidthtestwidthtestwidth', tel_phone: '10', ext: '12', fax_number: '1', cell_phone: '', mail: '1', status: '1' },
    //  { vendor: 'ford', vendor_name: '1', person: 'mondeo', address: '32000', tel_phone: '10', ext: '12', fax_number: '1', cell_phone: '', mail: '2', status: '1' },
    //  { vendor: 'porsche', vendor_name: '1', person: 'boxter', address: '72000', tel_phone: '10', ext: '12', fax_number: '1', cell_phone: '', mail: '3', status: '1' }
    //];
  }

  onRefreshMatnr() {
    this._srmRfqService.GetMatnr().subscribe(result => {
      console.log(result);
      this.OriMatnrList = result;
      console.log(this.OriMatnrList);
      this._searchObject.matnr = this.searchForm.value["matnr"] == null ? "" : this.searchForm.value["matnr"];
      var temp = this.OriMatnrList.filter(item => item.matnr.toUpperCase().indexOf(this._searchObject.matnr.toUpperCase()) >= 0);
      console.log(temp);
      this.MatnrList = [];
      for (var i = 0; i < temp.length; i++) {
        this.MatnrList.push({ label: temp[i].matnr, value: temp[i].matnrId });
      }
    });
  }

  onRefreshVendor() {
    this._srmRfqService.GetVendor().subscribe(result => {
      console.log(result);
      this.OriVendorList = result;
      console.log(this.OriVendorList);
      this._searchObject.vendor = this.searchForm.value["vendor"] == null ? "" : this.searchForm.value["vendor"];
      var temp = this.OriVendorList.filter(item => item.vendor.toUpperCase().indexOf(this._searchObject.vendor.toUpperCase()) >= 0);
      console.log(temp);
      this.VendorList = [];
      for (var i = 0; i < temp.length; i++) {
        this.VendorList.push({ label: temp[i].vendor, value: temp[i].vendorId });
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
    if (this.H.RFQID != undefined) {
      this.name = this.H.CREATEBY;
      console.log('GetRfqData');
      this._srmRfqService.GetRfqData(this.H.RFQID).subscribe(result => {
        console.log(result);
        console.log(result["h"]);
        this.H = result["h"];
        var c = new Date(this.H.createDate);
        this.H.C_Date = c.getFullYear() + '-' + (c.getMonth() + 1) + '-' + c.getDate();
        this.rowData_MATNR = result["m"];
        this.rowData_VENDOR = result["v"];
      });
    } else {
      this.name = this._storageService.Name;
      this.H.CREATEBY = this._storageService.userName;
      this.H.STATUS = 0;
    }
    this.H.LASTUPDATEBY = this._storageService.userName;
    console.log(this.name);
    $($('.listbox')[1]).hide();
    this.searchForm = this._formBuilder.group({
      matnr: [null],
      vendor:[null]
    });
    //this.onRefreshMatnr();
  }

  getRfqData() {
    var rfq = {
      h: null, m: null, v: null
    }
    var date = new Date();
    rfq.h = {
      RFQNUM: 'test1',
      STATUS: 0,
      SOURCER: '137680',
      DEADLINE: '2021/7/16',
      //CREATEDATE: this.datepipe.transform(date, 'yyyy-MM-dd'),
      CREATEBY: '137680',
      //LASTUPDATE_DATE: Date(),
      LASTUPDATEBY: '137680'
    }
    rfq.m = [];
    rfq.v = [];
  }

  add(title: TemplateRef<{}>, content: TemplateRef<{}>) {
    this.tplModal = this._modalService.create({
      nzTitle: title,
      nzContent: content,
      nzFooter: null,
      nzClosable: true,
      nzMaskClosable: false
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

  submitMatnrList() {
    console.log(this.rowData_MATNR);
    for (var i = 0; i < this.MatnrList.length; i++) {
      if (this.MatnrList[i].checked == true) {
        console.log(this.MatnrList[i].label);
        if (this.rowData_MATNR.filter(item => item.matnr.toUpperCase().indexOf(this.MatnrList[i].label.toUpperCase()) >= 0).length == 0) {
          this.rowData_MATNR.push(this.OriMatnrList.filter(item => item.matnr == this.MatnrList[i].label)[0]);
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
        if (this.rowData_VENDOR.filter(item => item.vendor.toUpperCase().indexOf(this.VendorList[i].label.toUpperCase()) >= 0).length == 0) {
          this.rowData_VENDOR.push(this.OriVendorList.filter(item => item.vendor == this.VendorList[i].label)[0]);
        }
      }
    }
    this.gridApi_VENDOR.setRowData(this.rowData_VENDOR);
    console.log(this.rowData_VENDOR);
    this.tplModal.close();
  }


  cancelEdit() {
    this.tplModal.close();
  }
  searchMatnrList() {
    this.onRefreshMatnr();
  }
  searchVendorList() {
    this.onRefreshVendor();
  }

  save() {
    var rfq = {
      h: null, m: null, v: null
    }
    var date = new Date();
    this.H.SOURCER = "137680";

    rfq.h = this.H;
    if (this.H.deadline != undefined) {
      var deadline = new Date(this.H.deadline);
      rfq.h.deadLine = deadline.getFullYear() + '-' + (deadline.getMonth() + 1) + '-' + deadline.getDate();
    } else {
      rfq.h.deadLine = null;
    }
    rfq.h.LASTUPDATEBY = this._storageService.userName;
    //rfq.h = {
    //  STATUS: 0,
    //  SOURCER: "TEST",
    //  DEADLINE: this.deadLine,
    //  //CREATEDATE: this.datepipe.transform(date, 'yyyy-MM-dd'),
    //  //LASTUPDATE_DATE: Date(),
    //  //LASTUPDATE_BY: this._storageService.userName,
    //};
    console.log(this.gridApi);
    this.gridApi.forEachNode(node => console.log(node));
    rfq.m = [];
    rfq.v = [];
    //this.rowData_MATNR.forEach(row => rfq.m.push(row));
    //this.rowData_VENDOR.forEach(row => rfq.v.push(row));
    this.gridApi.forEachNode(node => rfq.m.push(node.data));
    this.gridApi_VENDOR.forEachNode(node => rfq.v.push(node.data));
    this._srmRfqService.SAVE(rfq).subscribe(result => {
      console.log(result);
    });
  }

}
