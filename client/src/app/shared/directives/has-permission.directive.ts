import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';
import { AuthService } from '../../services/auth.service';

@Directive({
  selector: '[appHasPermission]'
})
export class HasPermissionDirective {
  private permissions: string[] = [];

  constructor(
    private authService: AuthService,
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef
  ) {
    this.authService.userPermissions$.subscribe(permissions => {
      this.permissions = permissions;
      this.updateView();
    });
  }

  @Input() set appHasPermission(permission: string) {
    if (this.permissions.includes(permission)) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainer.clear();
    }
  }

  private updateView() {
    this.viewContainer.clear();
  }
}
