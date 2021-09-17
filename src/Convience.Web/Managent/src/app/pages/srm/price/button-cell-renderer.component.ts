// Author: T4professor

import { Component } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';

@Component({
  selector: 'app-button-renderer',
  template: `
<ng-container *ngIf="display">
    <button *canOperate="'RFQ_ACTION'" nz-button type="button" nzShape="circle" style="display:{{display}}" (click)="onClick($event)"><i nz-icon nzType="edit"></i></button>
</ng-container>
`
})
export class ButtonRendererComponent implements ICellRendererAngularComp {

  params;
  label: string;
  display: boolean;

  agInit(params): void {
    this.params = params;
    this.label = this.params.label || null;
    this.display = this.params.data.rfqNum ? this.params.data.isStarted ? this.params.data.canEdit? true : false : true : false;
  }

  refresh(params?: any): boolean {
    return true;
  }

  onClick($event) {
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
}
