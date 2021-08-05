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
    { field: '報價單號', resizable: true},
    { field: '料號', resizable: true},
    { field: '詢價單號', resizable: true},
    { field: '建立日期', resizable: true },
    { field: '建立人員', resizable: true},
    { field: '最後異動日期', resizable: true},
    { field: '最後異動人員', resizable: true },
    { field: '狀態', resizable: true}
  ];

  rowData = [
    { 序號: '1', 報價單號: 'QOT0000001', 料號: 'ZMMXX', 詢價單號: 'RFQ0000001', 建立日期: '2021/07/14', 建立人員: 'A', 最後異動日期: '2021/07/14', 最後異動人員: 'A', 狀態: 'New'},
    { 序號: '2',報價單號: 'QOT0000002', 料號: 'ZMMX1', 詢價單號: 'RFQ0000001', 建立日期: '2021/07/14', 建立人員: 'B', 最後異動日期: '2021/07/14', 最後異動人員: 'B', 狀態: 'New' },
    { 序號: '3',報價單號: 'QOT0000003', 料號: 'ZMMX2', 詢價單號: 'RFQ0000001', 建立日期: '2021/07/14', 建立人員: 'C', 最後異動日期: '2021/07/14', 最後異動人員: 'C', 狀態: 'New' },
    { 序號: '4',報價單號: 'QOT0000004', 料號: 'ZMMX3', 詢價單號: 'RFQ0000001', 建立日期: '2021/07/14', 建立人員: 'D', 最後異動日期: '2021/07/14', 最後異動人員: 'D', 狀態: 'New' },
  ];
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


}
