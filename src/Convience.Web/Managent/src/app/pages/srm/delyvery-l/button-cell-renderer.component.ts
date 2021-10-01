// Author: T4professor

import { Component } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';

@Component({
  selector: 'edit-button',
  template: `
  <ng-container *ngIf="display">
    <button *canOperate="'DO_UPDATE'" nz-button nz-tooltip nzTooltipTitle="編輯" type="button" nzShape="circle" style="display:{{display}}" (click)="onEditClick($event)"><i nz-icon nzType="edit"></i></button>
    <button *canOperate="'DO_ADD'" nz-button nz-tooltip nzTooltipTitle="新增" type="button" nzShape="circle" style="display:{{display}}, margin-left:10px" (click)="onAddClick($event)"><i nz-icon nzType="plus"></i></button>
    <button *canOperate="'DO_DELETE'" nz-button nz-tooltip nzTooltipTitle="刪除" type="button" nzShape="circle" style="display:{{display}}, margin-left:10px" (click)="onDeleteClick($event)"><i nz-icon nzType="minus"></i></button>
  </ng-container>`
})
export class EditButtonComponent implements ICellRendererAngularComp {

  params;
  label: string;
  display: boolean;

  agInit(params): void {
    this.params = params;
    this.label = this.params.label || null;
    this.display = true;//this.params.data.rfqNum ? this.params.data.isStarted ? false : true :false ;
  }

  refresh(params?: any): boolean {
    return true;
  }

  onEditClick($event) {
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
