//import { Component, OnInit } from '@angular/core';
import { GridOptions } from 'ag-grid-community';
import { Component, OnInit, ViewEncapsulation, OnDestroy } from '@angular/core';
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { MatDialog, MatDialogConfig } from "@angular/material/dialog";

@Component({
  selector: 'app-qotlist',
  templateUrl: './qotlist.component.html',
  styleUrls: ['./qotlist.component.less']
})

export class QotlistComponent implements OnInit {
  /**/
  //columnDefs;
  //rowData: [];
  //columnApi;
  //gridApi;
  
  detailCellRendererParams;
  detailCellRendererParams2;
  defaultColDef;
  rowSelection = "multiple";
  /**/
  public gridApi;
  rowData_Qot;
  columnApi;
  constructor(
    //private httpClient: HttpClient, private dialog: MatDialog
  ) 
  {
  }
  ngOnInit(): void {
    throw new Error('Method not implemented.');
  }

 
  columnDefs = [
    { field: '序號', resizable: true},
    { field: '詢價單號', resizable: true},
    { field: '狀態', resizable: true},
    { field: '建立日期', resizable: true },
    { field: '建立人員', resizable: true},
    { field: '最後異動日期', resizable: true},
    { field: '最後異動人員', resizable: true },
    
  ];
 
  gridOptions = {
    columnDefs:this.columnDefs,
    masterDetail: true,
    rowSelection: 'multiple',
    suppressRowClickSelection: true,
    enableRangeSelection: true,
    pagination: true,
    paginationAutoPageSize: true,
    detailCellRendererParams: {
      detailGridOptions: {
        columnDefs1: [
          { field: '報價單號' },
          { field: '報價單號' },
          { field: '狀態', minWidth: 150 },
          { field: '料號' },
          { field: '建立日期', minWidth: 150 },
          { field: '建立人員', minWidth: 150 },
          { field: '最後異動日期', minWidth: 150 },
          { field: '最後異動人員', minWidth: 150 },
        ],
        defaultColDef: {
          flex: 1,
        },
      },

    },
  }

  rowData = [
    { 序號: '1',  詢價單號: 'RFQ0000001', 建立日期: '2021/07/14', 建立人員: 'A', 最後異動日期: '2021/07/14', 最後異動人員: 'A', 狀態: '啟動'},
    { 序號: '2',  詢價單號: 'RFQ0000001', 建立日期: '2021/07/14', 建立人員: 'B', 最後異動日期: '2021/07/14', 最後異動人員: 'B', 狀態: '啟動' },
    { 序號: '3',  詢價單號: 'RFQ0000001', 建立日期: '2021/07/14', 建立人員: 'C', 最後異動日期: '2021/07/14', 最後異動人員: 'C', 狀態: '啟動' },
    { 序號: '4',  詢價單號: 'RFQ0000001', 建立日期: '2021/07/14', 建立人員: 'D', 最後異動日期: '2021/07/14', 最後異動人員: 'D', 狀態: '啟動' },
  ];
  /* */
  /* */
  
  private selectedRows = [];
  
  //onRowClicked() {
  //  alert(1);
  //}
  onRowClicked(event) {
  //event.data The selected row data, event.event is the mouse event, etc. You can log to see for yourself
    console.log('a row was clicked', event);
    alert(event.data["報價單號"])
    //alert(event.data[0].序號)
    //var itxst = JSON.stringify(event.data);
    //alert(itxst);
  }
  onGridReady(params) {
    this.gridApi = params.api;
    this.columnApi = params.columnApi;
    this.gridApi.sizeColumnsToFit();
  }
  gridOptions1 = {
    // enable Master / Detail
    masterDetail: true,

    // the first Column is configured to use agGroupCellRenderer
    columnDefs: [
        { field: 'name', cellRenderer: 'agGroupCellRenderer' },
        { field: 'account' }
    ],

    // provide Detail Cell Renderer Params
    detailCellRendererParams: {
        // provide the Grid Options to use on the Detail Grid
        detailGridOptions: {
            columnDefs: [
                { field: 'callId' },
                { field: 'direction' },
                { field: 'number'}
            ]
        },
        // get the rows for each Detail Grid
        getDetailRowData: params => {
            params.successCallback(params.data.callRecords);
        }
    },

    // other grid options ...
}

}
