import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
@Component({
  selector: 'app-po',
  templateUrl: './po.component.html',
  styleUrls: ['./po.component.less']
})
export class PoComponent implements OnInit {
  gridApi;
  gridColumnApi;
  masterDetail;
  columnDefs;
  defaultColDef;
  detailCellRendererParams;
  rowData: any;
  constructor(private http: HttpClient) {
    this.masterDetail = true;
    this.columnDefs = [
      {
        field: 'name',
        cellRenderer: 'agGroupCellRenderer',
      },
      { field: 'account' },
      { field: 'calls' },
      {
        field: 'minutes',
        valueFormatter: "x.toLocaleString() + 'm'",
      },
    ];
    this.defaultColDef = { flex: 1 };
    this.detailCellRendererParams = {
      detailGridOptions: {
        columnDefs: [
          { field: 'callId' },
          { field: 'direction' },
          {
            field: 'number',
            minWidth: 150,
          },
          {
            field: 'duration',
            valueFormatter: "x.toLocaleString() + 's'",
          },
          {
            field: 'switchCode',
            minWidth: 150,
          },
        ],
        defaultColDef: { flex: 1 },
      },

      getDetailRowData: function (params) {
        params.successCallback(params.data.callRecords);

      },
    };
    console.info(this.detailCellRendererParams);
  }

  ngOnInit(): void {
  }
  onFirstDataRendered(params) {
    setTimeout(function () {
      params.api.getDisplayedRowAtIndex(1).setExpanded(true);
    }, 0);
  }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;

    this.http
      .get('https://www.ag-grid.com/example-assets/master-detail-data.json')
      .subscribe((data) => {
        this.rowData = data;
        console.info(data);
      });
  }
}
