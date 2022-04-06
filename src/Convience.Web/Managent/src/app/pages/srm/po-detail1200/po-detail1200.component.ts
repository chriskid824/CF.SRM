import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { SrmPoService } from '../../../business/srm/srm-po.service';
import { SrmDeliveryService } from '../../../business/srm/srm-delivery.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { DateFilterModel } from 'ag-grid-community';
import { StorageService } from 'src/app/services/storage.service';

@Component({
  selector: 'app-po-detail1200',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './po-detail1200.component.html',
  styleUrls: ['./po-detail1200.component.less']
})
export class PoDetail1200Component implements OnInit {
  gridApi;
  gridColumnApi;
  columnDefs_1200;
  columnDefs_3100;
  columnDefs_both;
  excelStyles;
  autoGroupColumnDef;
  defaultColDef;
  rowSelection;
  rowGroupPanelShow;
  pivotPanelShow;
  paginationPageSize;
  paginationNumberFormatter;
  rowData;
  searchForm: FormGroup = new FormGroup({});
  deliverydate;
  org;
  constructor(private _formBuilder: FormBuilder, private http: HttpClient, private _srmPoService: SrmPoService, private _srmDeliveryService: SrmDeliveryService, private _storageService: StorageService) {
    this.columnDefs_1200 = [
      {
        headerName: '物料',
        field: 'Matnr',
        // checkboxSelection: checkboxSelection,
        // headerCheckboxSelection: headerCheckboxSelection,
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
          console.log([params.data.Org, params.data.VendorId]);
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
        field: 'OriginalDate',
        valueGetter: function (params) {
          if (params.data.OriginalDate == null) return "";
          var date = new Date(params.data.OriginalDate);
          return `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}`;
        }
      },
      {
        headerName: '請單本次需求日期',
        field: 'DeliveryDate',
        valueGetter: function (params) {
          if (params.data.DeliveryDate == null) return "";
          var date = new Date(params.data.DeliveryDate);
          return `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}`;
        },
        cellStyle: function (params) {
          var date = new Date(params.value);
          if (params.data.DeliveryDate != params.data.OriginalDate) {
            return { color: 'red' };
          }
        },
        cellClassRules: {
          redFont: params => params.data.DeliveryDate != params.data.OriginalDate,
        },
      },
      {
        headerName: '廠商交貨日期',
        field: 'ReplyDeliveryDate',
        valueGetter: function (params) {
          if (params.data.ReplyDeliveryDate == null) return "";
          var date = new Date(params.data.ReplyDeliveryDate);
          return `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}`;
        },
        cellStyle: function (params) {
          var date = new Date(params.value);
          if (params.data.ReplyDeliveryDate != params.data.DeliveryDate) {
            return { color: 'blue' };
          }
        },
        cellClassRules: {
          blueFont: params => params.data.ReplyDeliveryDate != params.data.DeliveryDate,
        },
      },
      {
        headerName: '差異天數',
        field: 'DiffDays',
        valueGetter: function (params) {
          if (params.data.ReplyDeliveryDate != null && params.data.DeliveryDate != null) {
            var date1 = params.data.ReplyDeliveryDate.substr(0, 10);
            var date2 = params.data.DeliveryDate.substr(0, 10);
            date1 = new Date(date1);
            date2 = new Date(date2);
            if (date1 >= date2) {
              return Math.abs(date1 - date2) / (1000 * 3600 * 24);
            }
            else {
              return '-' + Math.abs(date1 - date2) / (1000 * 3600 * 24);
            }
          }
          else {
            return "";
          }
        },
      },
      {
        headerName: '交貨狀態',
        field: 'PoStatus',
        valueGetter: function (params) {
          if (params.data.ReplyDeliveryDate != null && params.data.DeliveryDate != null) {
            var date1 = params.data.ReplyDeliveryDate.substr(0, 10);
            var date2 = params.data.DeliveryDate.substr(0, 10);
            if (date1 > date2) {
              return '交貨延遲'
            }
            else if (date1 < date2) {
              return '交貨提前'
            }
            else {
              return '交貨相符';
            }
          }
          else { return ""; }
        },
      },
      {
        headerName: '備註內文',
        field: 'OtherDesc',
      },
      {
        headerName: '儲存地點說明',
        field: 'Storage',
        valueGetter: function (params) {
          if (params.data.Storage == '05Z1') {
            return '服務倉'
          }
          return '一般倉';
        },
      },
      {
        headerName: '特殊製程報告',
        field: 'InspectionReport',
      },
      {
        headerName: '附SIP',
        field: 'SipReport',
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
        headerName: '下單承辦人員',
        field: 'EkgryDesc',
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
    this.excelStyles = [
      {
        id: "redFont",
        font: {
          color: "#FF0000"
        }
      },
      {
        id: "blueFont",
        font: {
          color: "#0000FF"
        }
      },
    ]
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

    this.columnDefs_3100 = [
      {
        headerName: '追蹤號碼',
        field: 'Bednr',
      },
      {
        headerName: '請購單-項次',
        field: 'Ebban',
      },
      {
        headerName: '採購單-項次',
        field: 'PoNum',
        minWidth: 170,
        valueGetter: function (params) {
          console.log([params.data.Org, params.data.VendorId]);
          if (params.data.PoNum == undefined) { return ""; }
          return params.data.PoNum + '-' + params.data.PoLId;
        },
      },
      //??
      {
        headerName: '工單號碼',
        field: 'WoNum',
      },
      {
        headerName: '物料',
        field: 'Matnr',
        // checkboxSelection: checkboxSelection,
        // headerCheckboxSelection: headerCheckboxSelection,
      },
      {
        headerName: '短文',
        field: 'Description',
      },
      {
        headerName: '正確精密物料版次',
        field: 'Wzb04',
      },
      {
        headerName: '採購單數量',
        field: 'Qty',
      },
      {
        headerName: '採購單單價',
        field: 'Price',
      },
      {
        headerName: '請單本次需求日期',
        field: 'DeliveryDate',
        valueGetter: function (params) {
          if (params.data.DeliveryDate == null) return "";
          var date = new Date(params.data.DeliveryDate);
          return `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}`;
        },
        /*cellStyle:function (params) {
          var date=new Date(params.value);  
          if (params.data.DeliveryDate!=params.data.OriginalDate) {    
            return {color: 'red'};
          }
        },
        cellClassRules: {
          redFont: params => params.data.DeliveryDate!=params.data.OriginalDate,
        },*/
      },
      {
        headerName: '採購交期回覆',
        field: 'Neindt',
      },
      {
        headerName: '備註內文',
        field: 'Tdline',
      },
      {
        headerName: '刻號',
        field: 'Zpano',
      },
      {
        headerName: '採單文件日期',
        field: 'Bedat',
      },
      {
        headerName: '待交貨',
        field: 'RemainQty',
        hide: 'true',
      },
      {
        headerName: '工單可交數',
        valueGetter: "0",
        hide: 'true',
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
        headerName: '原始需求日期',
        field: 'OriginalDate',
        valueGetter: function (params) {
          if (params.data.OriginalDate == null) return "";
          var date = new Date(params.data.OriginalDate);
          return `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}`;
        },
        hide: 'true',
      },

      {
        headerName: '廠商交貨日期',
        field: 'ReplyDeliveryDate',
        valueGetter: function (params) {
          if (params.data.ReplyDeliveryDate == null) return "";
          var date = new Date(params.data.ReplyDeliveryDate);
          return `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}`;
        },
        cellStyle: function (params) {
          var date = new Date(params.value);
          if (params.data.ReplyDeliveryDate != params.data.DeliveryDate) {
            return { color: 'blue' };
          }
        },
        cellClassRules: {
          blueFont: params => params.data.ReplyDeliveryDate != params.data.DeliveryDate,
        },
        hide: 'true',
      },

      {
        headerName: '儲存地點說明',
        field: 'Storage',
        valueGetter: function (params) {
          if (params.data.Storage == '05Z1') {
            return '服務倉'
          }
          return '一般倉';
        },
        hide: 'true',
      },
      {
        headerName: '特殊製程報告',
        hide: 'true',
      },
      {
        headerName: '附SIP',
        hide: 'true',
      },
      {
        headerName: '供應商代碼',
        field: 'VendorId',
        hide: 'true',
      },
      {
        headerName: '供應商名稱',
        field: 'VendorName',
        hide: 'true',
      },
      {
        headerName: '下單承辦人員',
        field: 'EkgryDesc',
        hide: 'true',
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

    this.columnDefs_both = [
      {
        headerName: '物料',
        field: 'Matnr',
        // checkboxSelection: checkboxSelection,
        // headerCheckboxSelection: headerCheckboxSelection,
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
          console.log([params.data.Org, params.data.VendorId]);
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
        field: 'OriginalDate',
        valueGetter: function (params) {
          if (params.data.OriginalDate == null) return "";
          var date = new Date(params.data.OriginalDate);
          return `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}`;
        }
      },
      {
        headerName: '請單本次需求日期',
        field: 'DeliveryDate',
        valueGetter: function (params) {
          if (params.data.DeliveryDate == null) return "";
          var date = new Date(params.data.DeliveryDate);
          return `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}`;
        },
        cellStyle: function (params) {
          var date = new Date(params.value);
          if (params.data.DeliveryDate != params.data.OriginalDate) {
            return { color: 'red' };
          }
        },
        cellClassRules: {
          redFont: params => params.data.DeliveryDate != params.data.OriginalDate,
        },
      },
      {
        headerName: '廠商交貨日期',
        field: 'ReplyDeliveryDate',
        valueGetter: function (params) {
          if (params.data.ReplyDeliveryDate == null) return "";
          var date = new Date(params.data.ReplyDeliveryDate);
          return `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}`;
        },
        cellStyle: function (params) {
          var date = new Date(params.value);
          if (params.data.ReplyDeliveryDate != params.data.DeliveryDate) {
            return { color: 'blue' };
          }
        },
        cellClassRules: {
          blueFont: params => params.data.ReplyDeliveryDate != params.data.DeliveryDate,
        },
      },
      {
        headerName: '差異天數',
        field: 'DiffDays',
        valueGetter: function (params) {
          if (params.data.ReplyDeliveryDate != null && params.data.DeliveryDate != null) {
            var date1 = params.data.ReplyDeliveryDate.substr(0, 10);
            var date2 = params.data.DeliveryDate.substr(0, 10);
            date1 = new Date(date1);
            date2 = new Date(date2);
            if (date1 >= date2) {
              return Math.abs(date1 - date2) / (1000 * 3600 * 24);
            }
            else {
              return '-' + Math.abs(date1 - date2) / (1000 * 3600 * 24);
            }
          }
          else {
            return "";
          }
        },
      },
      {
        headerName: '交貨狀態',
        field: 'PoStatus',
        valueGetter: function (params) {
          if (params.data.ReplyDeliveryDate != null && params.data.DeliveryDate != null) {
            var date1 = params.data.ReplyDeliveryDate.substr(0, 10);
            var date2 = params.data.DeliveryDate.substr(0, 10);
            if (date1 > date2) {
              return '交貨延遲'
            }
            else if (date1 < date2) {
              return '交貨提前'
            }
            else {
              return '交貨相符';
            }
          }
          else { return ""; }
        },
      },
      {
        headerName: '儲存地點說明',
        field: 'Storage',
        valueGetter: function (params) {
          if (params.data.Storage == '05Z1') {
            return '服務倉'
          }
          return '一般倉';
        },
      },
      {
        headerName: '特殊製程報告',
        field: 'InspectionReport',
      },
      {
        headerName: '附SIP',
        field: 'SipReport',
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
        headerName: '下單承辦人員',
        field: 'EkgryDesc',
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
      {
        headerName: '追蹤號碼',
        field: 'Bednr',
      },
      {
        headerName: '請購單-項次',
        field: 'Ebban',
      },
      {
        headerName: '工單號碼',
        field: 'WoNum',
      },
      {
        headerName: '正確精密物料版次',
        field: 'Wzb04',
      },
      {
        headerName: '採購交期回覆',
        field: 'Neindt',
      },
      {
        headerName: '備註內文',
        field: 'Tdline',
      },
      {
        headerName: '刻號',
        field: 'Zpano',
      },
      {
        headerName: '採單文件日期',
        field: 'Bedat',
      },
    ];

  }

  ngOnInit(): void {
    console.log('--------------------------------------------');
    console.log(this._storageService);
    this.searchForm = this._formBuilder.group({
      PO_NUM: [null],
      STATUS: [1],
      EkgryDesc: [null],
      replyDeliveryDate_s: [null],
      replyDeliveryDate_e: [null],
    });
    var query = {
       srmvendor:this._storageService.userName
    }
    this.GetOrg(query);
  }
  // onPageSizeChanged(newPageSize) {
  //   var value = document.getElementById('page-size').value;
  //   this.gridApi.paginationSetPageSize(Number(value));
  // }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;
    const sortModel = [
      { colId: 'Matnr', sort: 'asc' },
      { colId: 'Description', sort: 'asc' },
      { colId: 'DeliveryDate', sort: 'asc' },
    ];
    this.gridApi.setSortModel(sortModel);
    this.getPoLList(null);
  }
  submitSearch() {
    this.refresh();
  }
  pageChange() {
    this.refresh();
  }
  refresh() {
    var query = {
      poNum: this.searchForm.value["PO_NUM"] == null ? "" : this.searchForm.value["PO_NUM"],
      status: "15",
      replyDeliveryDate_s: this.searchForm.value["ReplyDeliveryDate_s"] == null ? "" : this.searchForm.value["ReplyDeliveryDate_s"],
      replyDeliveryDate_e: this.searchForm.value["ReplyDeliveryDate_e"] == null ? "" : this.searchForm.value["ReplyDeliveryDate_e"],
    }
    this.getPoLList(query);
  }
  GetOrg(query)
  {
    if (query == null) {
      query = {
        srmvendor: ""
      }
    }
    this._srmPoService.GetOrg(query)
      .subscribe((result) => {
        console.info('---------------GetOrg----------------')
        console.info(result)
        this.org = result;
      });
  }
  getPoLList(query) {
    if (query == null) {
      query = {
        poNum: "",
        status: "15",
        replyDeliveryDate_s: null,
        replyDeliveryDate_e: null,
        onlysevendays: false,
      }
    }
    this._srmPoService.GetPoL(query)
      .subscribe((result) => {
        this.rowData = result;
        console.info()
      });
  }
  getSelectedRowData(event) {
    let selectedNodes = this.gridApi.getSelectedNodes();
    if (selectedNodes.length == 0) {
      alert('請先選擇要出貨的項目!');
      return;
    }
    let selectedData = selectedNodes.map(node => node.data);
    var query = {
      date: this.deliverydate,
      data: selectedData,
    }
    console.info(query);
    this._srmDeliveryService.AddDelivery(query)
      .subscribe((result) => {
        if (result == null) alert('出貨單生成成功');
        this.refresh();
      });
  }
  excel(event) {
    let selectedNodes = this.gridApi.getSelectedNodes();
    let selectedData = selectedNodes.map(node => node.data);
    var query = {
      date: this.deliverydate,
      data: selectedData,
    }
    console.info(query);
    this._srmDeliveryService.GetDeliveryExcel(query)
      .subscribe((result) => {
        if (result == null) alert('出貨單生成成功');
        this.refresh();
      });
  }
}
var checkboxSelection = function (params) {
  return params.columnApi.getRowGroupColumns().length === 0;
};
var headerCheckboxSelection = function (params) {
  return params.columnApi.getRowGroupColumns().length === 0;
};
function dateFormatter(data) {
  if (data.value == null) return "";
  var date = new Date(data.value);
  return `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}`;
}
const ragCellClassRules = {
  'redFonts': (params) => params.value === "一般倉",
};

