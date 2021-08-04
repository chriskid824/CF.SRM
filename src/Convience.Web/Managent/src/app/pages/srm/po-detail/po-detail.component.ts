import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { SrmPoService } from '../../../business/srm/srm-po.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
@Component({
  selector: 'app-po-detail',
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
  constructor(private http: HttpClient,private _srmPoService: SrmPoService) {
    this.columnDefs = [
      {
        headerName:'採購單識別碼',
        field: 'PoId',
        minWidth: 170,
        checkboxSelection: checkboxSelection,
        headerCheckboxSelection: headerCheckboxSelection,
      },
      {
        headerName:'採購單明細識別碼',
        field: 'PoLId',
      },

      {
        headerName:'料號識別碼',
        field: 'MantrId',
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
        editable: function(params){return false},
        valueFormatter:dateFormatter,
      },
      {
        headerName:'交貨地點',
        field: 'DeliveryPlace',
      },
      {
        headerName:'關鍵零組件/首件',
        field: 'CriticalPart',
      },
      {
        headerName:'檢驗時間',
        field: 'InspectionTime',
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
      editable: true,
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
  getPoLList(query){
    if(query==null)
    {
      query = {
        poNum: "",
        status: "0",
        buyer: "",
      }
    }
    this._srmPoService.GetPoL(query)
      .subscribe((result) => {
        this.rowData = result;
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
