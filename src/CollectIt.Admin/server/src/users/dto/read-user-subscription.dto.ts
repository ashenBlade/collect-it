export class ReadUserSubscriptionDto {
  readonly userId: number;
  readonly subscriptionId: number;
  readonly from: Date;
  readonly to: Date;
  readonly leftResourcesCount: number;
}
