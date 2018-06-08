using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMarket.Bases
{
    /*
     Source: https://www.investors.com/how-to-invest/investors-corner/emulex-hoisted-a-high-tight-signal-flag-before-its-breakout/

     High, tight flag bases clearly make this statement: "We're up-and-coming."

This vertical-looking pattern forms as a stock surges 100% to 120% in four to eight weeks. The stock then corrects just 10% to 25% in price for only three to five weeks. The ideal buy point is the high of the flag plus 10 cents.

The base is counterintuitive, because an investor's tendency is to buy low and sell high. But in a high, tight flag, a stock is seeing a queue of traders ready to buy high and sell even higher. The base shows just a small portion of investors taking profit; others are impatient to press ahead.

The bases have acted as early launching pads for leaders including Qualcomm (QCOM), JDS Uniphase, Taser International (TASR) and, almost exactly a century ago, Bethlehem Steel. Keep in mind, though, that successful flag patterns are rare.

They often form in the second half of a huge rally by a stock. Breakouts by Mellanox Technologies (MLNX) (its initial gain was shy of 100%) in June 2012 and Tesla Motors (TSLA) in July 2013 offer a couple of examples of this.

And, like any base, breakouts from high, tight flags don't always succeed. It's worth repeating this point: Good flags are rare. A breakout by NQ Mobile (NQ) rose in only three of six weeks following its September 2013 breakout. It then dived 87% in nine months. Today, the stock is trading near 2 a share.

Another volatile, but much more successful, example was Emulex late in 1999.  This was just before technology and internet stocks tanked in early 2000, and long before Avago Technologies, which is now Broadcom (AVGO), acquired the data network connectivity specialist for $606 million in May 2015.

The high, tight flag occurred late in Emulex' 21-month advance. The stock cleared an area of tight trade in late August 1999 and ran up 107% from 43.25 to 89.50 in just five weeks. That was the flagpole.

Shares corrected for three weeks, easing back but never even coming close to testing support at the 10-week moving average. The buy point was 89.60, cleared on Oct. 4 in volume surging 65% above its 50-day average. Emulex soared 118% in less than six weeks before action grew volatile, including a huge reversal on Jan. 3, 2000.

The stock then succumbed to the dot-com crash, which was dragging down the general market from the Nasdaq composite's peak on March 10.
        
By April 14, 2000, Emulex round-tripped all of its gains.

(A version of this column was first published on March 29, 2016. JDS Uniphase was split into two companies, Lumentum (LITE) and Viavi Solutions (VIAV), in 2015. Also, please follow Elliott on Twitter at @IBD_AElliott for more commentary on stocks and financial markets.)






     */
    class HighTightFlag : Base
    {
        public new String FindBase()
        {
            return "High Tight Flag";
        }
    }
}
