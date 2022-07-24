Create a private Facebook page.

Create a Facebook app & GET a short-lived user access token from the Graph API Explorer page: https://developers.facebook.com/tools/explorer/{YOUR_APP_ID}/

Include these permissions:

- pages_manage_posts
- pages_read_engagement
- permissions

Using that, GET a long-lived access token:
https://graph.facebook.com/oauth/access_token?grant_type=fb_exchange_token&client_id={YOUR_APP_ID}&client_secret={GET_THIS_FROM_THE_APP_PAGE_ON_FACEBOOK}&fb_exchange_token={SHORT_LIVED_TOKEN_FROM_ABOVE}

Using that, GET a page access token:
https://graph.facebook.com/{PAGE_ID_FROM_THE_PAGE_YOU_ARE_POSTING_TO}?fields=access_token&access_token={LONG_LIVED_TOKEN_FROM_ABOVE}

Using that, you can POST whatever you want:
https://graph.facebook.com/{YOUR_APP_NAME}/feed?message={WHATEVER_YOU_WANT_TO_POST}&access_token={PAGE_TOKEN_FROM_ABOVE}

Add the page token to your lambda as an environment variable named `PageToken`.
