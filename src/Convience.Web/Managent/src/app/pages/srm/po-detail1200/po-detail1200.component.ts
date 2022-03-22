import { Component, OnInit,ViewEncapsulation } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { SrmPoService } from '../../../business/srm/srm-po.service';
import { SrmDeliveryService } from '../../../business/srm/srm-delivery.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { DateFilterModel } from 'ag-grid-community';
@Component({
  selector: 'app-po-detail1200',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './po-detail1200.component.html',
  styleUrls: ['./po-detail1200.component.less']
})
export class PoDetail1200Component implements OnInit {
  gridApi;
  gridColumnApi;

  columnDefs;
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
  constructor(private _formBuilder: FormBuilder,private http: HttpClient,private _srmPoService: SrmPoService,private _srmDeliveryService: SrmDeliveryService) {
    this.columnDefs = [
      {
        headerName:'物料',
        field: 'Matnr',
        // checkboxSelection: checkboxSelection,
        // headerCheckboxSelection: headerCheckboxSelection,
      },
      {
        headerName:'短文',
        field: 'Description',
      },
      {
        headerName:'採購單-項次',
        field: 'PoNum',
        minWidth: 170,
        valueGetter: function (params) {
          if(params.data.PoNum==undefined){return "";}
          return params.data.PoNum+'-'+params.data.PoLId;
        },
      },
      {
        headerName:'採購單數量',
        field: 'Qty',
      },
      {
        headerName:'待交貨',
        field: 'RemainQty',
      },
      {
        headerName:'工單可交數',
        valueGetter:"0",
      },
      {
        headerName:'此次交貨數量',
        field: 'DeliveryQty',
        editable:true,
        valueGetter: function (params) {
          console.info(params);
          if(params.data.DeliveryQty>params.data.RemainQty)
          {
            params.data.DeliveryQty=params.data.RemainQty;
          }
          return params.data.DeliveryQty;
        },
        hide:'true',
      },
      {
        headerName:'採購單單價',
        field: 'Price',
      },
      {
        headerName:'原始需求日期',
        field: 'DeliveryDate',
        valueFormatter:dateFormatter
      },
      {
        headerName:'請單本次需求日期',
        field: 'DeliveryDate',
        valueFormatter:dateFormatter
      },
      {
        headerName:'廠商交貨日期',
        field: 'ReplyDeliveryDate',
        valueFormatter:dateFormatter,
      },
      {
        headerName:'備註內文',
        field: 'OtherDesc',
      },
      {
        headerName:'儲存地點說明',
        field: 'Storage',
        valueGetter: function (params) {
          console.info(params);
          if(params.data.Storage=='05Z1')
          {
            return '服務倉'
          }
          return '非服務倉';
        },
      },
      {
        headerName:'特殊製程報告',
      },
      {
        headerName:'附SIP',
      },
      {
        headerName:'供應商代碼',
        field: 'VendorId',
        hide:'true',
      },
      {
        headerName:'供應商名稱',
        field: 'VendorName',
      },
      {
        headerName:'下單承辦人員',
        field: 'EkgryDesc',
      },
      {
        headerName:'採購單識別碼',
        field: 'PoId',
        minWidth: 170,
        hide:'true',
      },
      {
        headerName:'採購單明細識別碼',
        field: 'PoLId',
        hide:'true',
      },

      {
        headerName:'狀態',
        field: 'StatusDesc',
        hide:'true',
      },

      {
        headerName:'採購單總金額',
        field: 'TotalAmount',
        hide:'true'
      },

      {
        headerName:'料號識別碼',
        field: 'MatnrId',
        hide:'true'
      },
      {
        headerName:'交貨地點',
        field: 'DeliveryPlace',
        hide:'true'
      },
      {
        headerName:'關鍵零組件',
        field: 'CriticalPart',
        hide:'true'
      },
      {
        headerName:'檢驗時間(天)',
        field: 'InspectionTime',
        hide:'true'
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
      PO_NUM: [null],
      STATUS: [1],
      EkgryDesc:[null],
      replyDeliveryDate_s: [null],
      replyDeliveryDate_e: [null],
    });
  }
  // onPageSizeChanged(newPageSize) {
  //   var value = document.getElementById('page-size').value;
  //   this.gridApi.paginationSetPageSize(Number(value));
  // }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;
    const sortModel = [
      {colId: 'Matnr', sort: 'desc'},
      {colId: 'Description', sort: 'desc'},
      {colId: 'DeliveryDate', sort: 'desc'},
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
  getPoLList(query){
    if(query==null)
    {
      query = {
        poNum: "",
        status: "15",
        replyDeliveryDate_s: null,
        replyDeliveryDate_e: null,
        onlysevendays:false,
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
    if(selectedNodes.length==0)
    {
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
      if(result==null) alert('出貨單生成成功');
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
      if(result==null) alert('出貨單生成成功');
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
  if(data.value==null) return "";
  var date=new Date(data.value);
  return `${date.getFullYear()}-${date.getMonth()+1}-${date.getDate()}`;
}
