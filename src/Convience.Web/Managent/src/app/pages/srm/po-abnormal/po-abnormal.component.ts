import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { SrmPoService } from '../../../business/srm/srm-po.service';


import { HttpClient } from '@angular/common/http';
import { NzDatePickerComponent } from 'ng-zorro-antd/date-picker';
import datepickerFactory from 'jquery-datepicker';
import datepickerJAFactory from 'jquery-datepicker/i18n/jquery.ui.datepicker-en-GB';


import { ViewSrmFileUploadRecordH, ViewSrmFileUploadRecordL } from '../model/File';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { MatDialog, MatDialogConfig } from "@angular/material/dialog";

import { FileModalComponent } from '../file-modal/file-modal.component';
import { ActivatedRoute } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzUploadFile } from 'ng-zorro-antd/upload';
import { SrmFileService } from 'src/app/business/srm/srm-file.service';
import { FileService } from 'src/app/business/file.service';

import { StorageService } from 'src/app/services/storage.service';


@Component({
  selector: 'app-po-abnormal',
  templateUrl: './po-abnormal.component.html',
  styleUrls: ['./po-abnormal.component.less']
})
export class PoAbnormalComponent implements OnInit {
  searchForm: FormGroup = new FormGroup({});
  page: number = 1;
  size: number = 2;
  total: number;
  isVisible = false;
  searchId;
  rowData: any;
  columnDefs;
  autoGroupColumnDef;
  defaultColDef;
  rowSelection;
  rowGroupPanelShow;
  pivotPanelShow;
  paginationPageSize;
  paginationNumberFormatter;
  constructor(private _formBuilder: FormBuilder, private http: HttpClient, private _srmPoService: SrmPoService
    , private _messageService: NzMessageService,
    private _modalService: NzModalService,
    private _srmFileService: SrmFileService,
    private _fileService: FileService,
    private dialog: MatDialog, private route: ActivatedRoute,
    private _storageService: StorageService) {
    this.columnDefs = [
      {
        headerName: '物料',
        field: 'Matnr',
      },
      {
        headerName: '短文',
        field: 'Description',
      },
      {
        headerName: '採購單-項次',
        field: 'PoNum',
        minWidth: 170,
        valueGetter: function (params) {
          if (params.data.PoNum == undefined) { return ""; }
          return params.data.PoNum + '-' + params.data.PoLId;
        },
      },
      {
        headerName: '採購單數量',
        field: 'Qty',
      },
      {
        headerName: '待交貨',
        field: 'RemainQty',
      },
      {
        headerName: '工單可交數',
        field: 'WoQty',
      },
      {
        headerName: '此次交貨數量',
        field: 'DeliveryQty',
        editable: true,
        valueGetter: function (params) {
          console.info(params);
          if (params.data.DeliveryQty > params.data.RemainQty) {
            params.data.DeliveryQty = params.data.RemainQty;
          }
          return params.data.DeliveryQty;
        },
        hide: 'true',
      },
      {
        headerName: '採購單單價',
        field: 'Price',
      },
      {
        headerName: '原始需求日期',
        field: 'DeliveryDate',
        valueFormatter: dateFormatter
      }, 
      {
        headerName: '請單本次需求日期',
        field: 'DeliveryDate',
        valueFormatter: dateFormatter
      },
      {
        headerName: '廠商交貨日期',
        field: 'ReplyDeliveryDate',
        valueFormatter: dateFormatter,
      },
      {
        headerName: '備註內文',
        field: 'Tdline',
      },
      {
        headerName: '儲存地點說明',
        field: 'Storage',
        valueGetter: function (params) {
          console.info(params);
          if (params.data.Storage == '05Z1') {
            return '服務倉'
          }
          return '一般倉';
        },
      },
      {
        headerName: '供應商代碼',
        field: 'VendorId',
        hide: 'true',
      },
      {
        headerName: '供應商名稱',
        field: 'VendorName',
      },
      {
        headerName: '採購單識別碼',
        field: 'PoId',
        minWidth: 170,
        hide: 'true',
      },
      {
        headerName: '採購單明細識別碼',
        field: 'PoLId',
        hide: 'true',
      },

      {
        headerName: '狀態',
        field: 'StatusDesc',
        hide: 'true',
      },

      {
        headerName: '採購單總金額',
        field: 'TotalAmount',
        hide: 'true'
      },

      {
        headerName: '料號識別碼',
        field: 'MatnrId',
        hide: 'true'
      },
      {
        headerName: '交貨地點',
        field: 'DeliveryPlace',
        hide: 'true'
      },
      {
        headerName: '關鍵零組件',
        field: 'CriticalPart',
        hide: 'true'
      },
      {
        headerName: '檢驗時間(天)',
        field: 'InspectionTime',
        hide: 'true'
      },
    ];
    this.autoGroupColumnDef = {
      headerName: 'Group',
      minWidth: 170,
      field: 'athlete',
      valueGetter: function (params) {
        if (params.node.group) {
          return params.node.key;
        } else {
          return params.data[params.colDef.field];
        }
      },
      headerCheckboxSelection: true,
      cellRenderer: 'agGroupCellRenderer',
      cellRendererParams: { checkbox: true },
    };
    this.defaultColDef = {
      editable: false,
      enableRowGroup: true,
      enablePivot: true,
      enableValue: true,
      sortable: true,
      resizable: true,
      filter: true,
      flex: 1,
      minWidth: 150,
    };
    this.rowSelection = 'multiple';
    this.rowGroupPanelShow = 'always';
    this.pivotPanelShow = 'always';
    this.paginationPageSize = 15;
    this.paginationNumberFormatter = function (params) {
      return '[' + params.value.toLocaleString() + ']';
    };
  }

  ngOnInit(): void {
    this.searchForm = this._formBuilder.group({
      PO_NUM: this.route.snapshot.paramMap.get('number'),
      srmvendor: [null],
      STATUS: [1]
    });
  }
  onGridReady(params) {
   
    this.getPoList(null);
  }
  submitSearch() {
    this.refresh();
  }
  refresh() {
    var query = {
      poNum: this.searchForm.value["PO_NUM"] == null ? "" : this.searchForm.value["PO_NUM"],
      srmvendor: this.searchForm.value["srmvendor"] == null ? "" : this.searchForm.value["srmvendor"],
      status: this.searchForm.value["STATUS"] == null ? "0" : this.searchForm.value["STATUS"],
      page: this.page,
      size: this.size
    }
    console.log('-----refresh-----')
    console.log(query)
    this.getPoList(query);
  }
  getPoList(query) {
    if (query == null) {
      query = {
        srmvendor: "",
        status: "0",
        page: this.page,
        size: this.size
      }
    }
    this._srmPoService.GetPoLAbnormal(query)
      .subscribe((result) => {
        this.rowData = result;
      });
  }
}
function dateFormatter(data) {
  if (data.value == null) return "";
  var date = new Date(data.value);
  return `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}`;
}
