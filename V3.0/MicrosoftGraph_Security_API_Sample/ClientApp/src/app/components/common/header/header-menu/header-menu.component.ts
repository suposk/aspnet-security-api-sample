import { Component } from '@angular/core';

@Component({
    selector: 'app-header-menu',
    templateUrl: './header-menu.component.html',
    styleUrls: ['./header-menu.component.css']
})
export class HeaderMenuComponent {
    public items = ["About"];

    public openMenu(): void { }
}
