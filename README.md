# GigMarket

## Roadmap & Progress

### Phase 1: Foundation (Weeks 1-4)
#### Sprint 1: The Skeleton & Auth (Current)
**Goal:** A user can Register, Login (Cookie Auth), and create a Seller Profile.

**Week 1:**
- [x] Project Setup (Solution, 4 Layers, Git).
- [x] Database: Design AppUser and SellerProfile entities (MSSQL).
- [x] Infrastructure: Implement Identity with cookie-based authentication.
- [x] API: Create AuthController (Register/Login endpoints).

**Week 2:**
- [x] Frontend: Setup Angular with HTTP interceptors (auth cookies).
- [x] Frontend: Create login and registreation page.
- [x] Frontend and backend communication.

#### Sprint 2: The "Gig" Engine
**Goal:** A Seller can create, edit, and delete Gigs with images.

**Week 3:**
- [x] Feature: "Become a Seller" page (Form to create SellerProfile).
- [x] UI: Basic Navbar (Login/Logout state).


**Week 4 (03.12):**
- [x] Frontend: "Create Gig" Wizard (Multi-step form).
- [x] Feature: Image Drag & Drop component.

### Phase 2: The Marketplace (Weeks 5-8)
#### Sprint 3: Discovery & Search
**Goal:** A Buyer can find gigs using search text and filters.

**Week 5:**
- [x] Frontend: Search Results page with filters.
- [x] Database: Create Gig entity (Title, Description, Category) and GigPackage (Basic/Std/Prem pricing).
- [x] API: GigsController (CRUD endpoints).
- [x] Infrastructure: Implement Azure Blobstorage.

**Week 6 (03.26):**
- [x] Frontend: Gig Detail Page.
- [x] UI: "My Gigs" dashboard card (Edit/Delete buttons).

#### Sprint 4: The Order System (Complex)
**Goal:** A Buyer can "purchase" a gig, creating an Order.

**Week 7:**
- [x] Database: Order entity (Status: Pending, InProgress, Completed).
- [x] API: OrdersController (Create Order, Mark as Delivered).
- [x] Feature: Stripe payment integration (Checkout flow + webhook handling).

**Week 8 (04.16):**
- [x] Feature: "Contact Seller" button (Drafts a message).
- [x] Real-time: Setup SignalR for Notifications.
- [x] Real-time: SignalR Chat Hub (1-on-1 messaging).
- [x] Database: Messages table.
- [x] Frontend: Inbox UI.

### Phase 3: Ecosystem & Polish (Weeks 9-12)
#### Sprint 5: Chat & Reviews
**Goal:** Users can talk in real-time and leave reviews.

**Week 9:**
- [ ] Frontend: Order Page (Timeline view: "Order Started" -> "Delivery Submitted").

**Week 10 (04.30):**
- [ ] Feature: Review System (Stars + Comment).
- [ ] Logic: Only allow review if Order Status == Completed.
- [ ] Logic: Calculate Seller's average rating automatically.

#### Sprint 6: Differentiators & Polish
**Goal:** Implement one standout feature and finalize production hardening.

**Week 11 (Optional Features):**
- [ ] Bug Fixes

**Week 12 (05.14):**
- [ ] Bug Fixes
