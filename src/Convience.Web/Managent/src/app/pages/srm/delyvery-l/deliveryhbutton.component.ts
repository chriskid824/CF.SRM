// Author: T4professor

import { Component } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';

@Component({
  selector: 'deliveryh-button',
  template: `
  <ng-container *ngIf="display">
    <button *canOperate="'DO_UPDATE'" nz-button nz-tooltip nzTooltipTitle="列印" type="button" nzShape="circle" style="display:{{display}}" (click)="onPrintClick($event)"><i nz-icon nzType="printer"></i></button>
    <button *canOperate="'DO_DELETE'" nz-button nz-tooltip nzTooltipTitle="刪除" type="button" nzShape="circle" style="display:{{display}}, margin-left:10px" (click)="onDeleteClick($event)"><i nz-icon nzType="minus"></i></button>
  </ng-container>`
})
export class DeliveryHButtonComponent implements ICellRendererAngularComp {

  params;
  label: string;
  display: boolean;

  agInit(params): void {
    this.params = params;
    this.label = this.params.label || null;
    this.display = this.params.data.Status==14;//this.params.data.rfqNum ? this.params.data.isStarted ? false : true :false ;
  }

  refresh(params?: any): boolean {
    return true;
  }

  onPrintClick($event) {
    //this.display=false;
    if (this.params.onClick instanceof Function) {
      // put anything into params u want pass into parents component
      const params = {
        event: $event,
        rowData: this.params.node.data
        // ...something
      }
      this.params.ondblclick(params);

    }
  }
  onAddClick($event) {
    //this.display=false;
    if (this.params.onClick instanceof Function) {
      // put anything into params u want pass into parents component
      const params = {
        event: $event,
        rowData: this.params.node.data
        // ...something
      }
      this.params.onClick(params);

    }
  }
  onDeleteClick($event) {
    if (this.params.onClick instanceof Function) {
      const params = {
        event: $event,
        rowData: this.params.node.data
      }
      this.params.oncancel(params);

    }
  }
}
