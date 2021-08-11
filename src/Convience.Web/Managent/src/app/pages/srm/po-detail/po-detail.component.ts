import { Component, OnInit,ViewEncapsulation } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { SrmPoService } from '../../../business/srm/srm-po.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
@Component({
  selector: 'app-po-detail',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './po-detail.component.html',
  styleUrls: ['./po-detail.component.less']
})
export class PoDetailComponent implements OnInit {
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
  constructor(private _formBuilder: FormBuilder,private http: HttpClient,private _srmPoService: SrmPoService) {
    this.columnDefs = [
      {
        headerName:'採購單號',
        field: 'PoNum',
        minWidth: 170,
        checkboxSelection: checkboxSelection,
        headerCheckboxSelection: headerCheckboxSelection,
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
        headerName:'供應商識別碼',
        field: 'VendorId',
        hide:'true',
      },
      {
        headerName:'供應商',
        field: 'VendorName',
      },
      {
        headerName:'狀態',
        field: 'Status',
        cellClassRules: {
          'rag-green': 'x == 12',
          'rag-amber': 'x == 11',
          'rag-red': 'x == 10',
        },
        valueFormatter:'switch(value){case 10 : return "待確認"; case 11 : return "已確認"; case 12 : return "已回覆"; default : return "未知";}'
      },
      {
        headerName:'採購單總金額',
        field: 'TotalAmount',
      },
      {
        headerName:'採購人員',
        field: 'Buyer',
      },
      {
        headerName:'料號識別碼',
        field: 'MatnrId',
        hide:'true'
      },
      {
        headerName:'料號',
        field: 'Matnr',
      },
      {
        headerName:'物料內文',
        field: 'Description',
      },
      {
        headerName:'數量',
        field: 'Qty',
      },
      {
        headerName:'未交貨數量',
        field: 'RemainQty',
      },
      {
        headerName:'此次交貨數量',
        field: 'DeliveryQty',
        editable:true
      },
      {
        headerName:'單價',
        field: 'Price',
      },
      {
        headerName:'交貨日期',
        field: 'DeliveryDate',
        valueFormatter:dateFormatter
      },
      {
        headerName:'廠商交貨日期',
        field: 'ReplyDeliveryDate',
        valueFormatter:dateFormatter,
      },
      {
        headerName:'交貨地點',
        field: 'DeliveryPlace',
        hide:'true'
      },
      {
        headerName:'關鍵零組件/首件',
        field: 'CriticalPart',
        hide:'true'
      },
      {
        headerName:'檢驗時間',
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
      minWidth: 100,
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
      BUYER:[null],
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
      status: this.searchForm.value["STATUS"] == null ? "0" : this.searchForm.value["STATUS"],
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
        status: "0",
        replyDeliveryDate_s: null,
        replyDeliveryDate_e: null,
      }
    }
    this._srmPoService.GetPoL(query)
      .subscribe((result) => {
        this.rowData = result;
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
    console.info(selectedData);
    this._srmPoService.AddDelivery(selectedData)
    .subscribe((result) => {
      if(result==null) alert('出貨單生成成功');
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
