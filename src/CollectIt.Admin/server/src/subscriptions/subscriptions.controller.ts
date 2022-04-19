import {Body, Controller, Get, Param, Post, Query} from '@nestjs/common';
import {SubscriptionsService} from "./subscriptions.service";
import {ResourceType} from "../common/resource-type";
import {ParseResourceTypePipe} from "../common/parse-resource-type.pipe";
import {Authorize} from "../auth/jwt-auth.guard";
import {AuthorizeAdmin} from "../auth/admin-jwt-auth.guard";

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
    async createSubscription(@Body() createSubscriptionDto) {
        throw new Error('Not implemented yet');
    }
}
