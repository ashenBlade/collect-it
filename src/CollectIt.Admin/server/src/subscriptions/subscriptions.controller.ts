import {Body, Controller, Get, Param, Post, Query} from '@nestjs/common';
import {SubscriptionsService} from "./subscriptions.service";
import {ResourceType} from "../common/resource-type";
import {ParseResourceTypePipe} from "../common/parse-resource-type.pipe";
import {Authorize} from "../auth/jwt-auth.guard";
import {AuthorizeAdmin} from "../auth/admin-jwt-auth.guard";
import {CreateSubscriptionDto} from "./dto/create-subscription.dto";

@Authorize()
@Controller('api/v1/subscriptions')
export class SubscriptionsController {
    constructor(private subscriptionsService: SubscriptionsService) { }

    @Get(':subscriptionId')
    async getSubscriptionById(@Param('subscriptionId') subscriptionId: number) {
        const subscriptions = await this.subscriptionsService.getSubscriptionById(subscriptionId);
        return subscriptions;
    }

    @AuthorizeAdmin()
    @Post('')
    async createSubscription(@Body() dto: CreateSubscriptionDto) {
        const subscription = await this.subscriptionsService.createSubscriptionAsync(
            dto.name,
            dto.description,
            dto.price,
            dto.monthDuration,
            dto.appliedResourceType,
            dto.maxResourcesCount,
            dto.active,
            dto.restriction?.restrictionType,
            dto.restriction?.authorId,
            dto.restriction?.sizeBytes,
            dto.restriction?.daysTo,
            dto.restriction?.daysAfter,
            dto.restriction?.tags
        );
        console.log(`Created subscription: ${JSON.stringify(subscription, null,  2)}`);
        return subscription;
    }
}
