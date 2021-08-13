// Author: T4professor

import { Component } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';

@Component({
  selector: 'app-button-renderer',
  template: `
    <button *ngIf="display" nz-button type="button" nzShape="circle" style="display:{{display}}" (click)="onClick($event)"><i nz-icon nzType="edit"></i></button>
    `
})
export class ButtonRendererComponent implements ICellRendererAngularComp {

  params;
  label: string;
  display: boolean;

  agInit(params): void {
    console.log('b');
    this.params = params;
    this.label = this.params.label || null;
    this.display = this.params.data.rfqNum? true : false;
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
