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
- [ ] Feature: "Become a Seller" page (Form to create SellerProfile).
- [ ] UI: Basic Navbar (Login/Logout state) and Dashboard layout.
- [ ] Database: Create Gig entity (Title, Description, Category) and GigPackage (Basic/Std/Prem pricing).
- [ ] API: GigsController (CRUD endpoints).
- [ ] Infrastructure: Implement IPhotoService (Upload images to AWS or Azure — TBD).

**Week 4:**
- [ ] Frontend: "Create Gig" Wizard (Multi-step form).
- [ ] Feature: Image Drag & Drop component.
- [ ] UI: "My Gigs" dashboard card (Edit/Delete buttons).

### Phase 2: The Marketplace (Weeks 5-8)
#### Sprint 3: Discovery & Search
**Goal:** A Buyer can find gigs using search text and filters.

**Week 5:**
- [ ] Frontend: Search Results page with filters (Price slider, Dropdowns).

**Week 6:**
- [ ] Frontend: Gig Detail Page (The most important page).
- [ ] UI: Pricing Package Selector (Tabs for Basic/Standard/Premium).
- [ ] Feature: "Contact Seller" button (Drafts a message).

#### Sprint 4: The Order System (Complex)
**Goal:** A Buyer can "purchase" a gig, creating an Order.

**Week 7:**
- [ ] Database: Order entity (Status: Pending, InProgress, Completed).
- [ ] API: OrdersController (Create Order, Mark as Delivered).
- [ ] Feature: Stripe payment integration (Checkout flow + webhook handling).

**Week 8:**
- [ ] Real-time: Setup SignalR for Notifications.
- [ ] Frontend: Order Page (Timeline view: "Order Started" -> "Delivery Submitted").

### Phase 3: Ecosystem & Polish (Weeks 9-12)
#### Sprint 5: Chat & Reviews
**Goal:** Users can talk in real-time and leave reviews.

**Week 9:**
- [ ] Real-time: SignalR Chat Hub (1-on-1 messaging).
- [ ] Database: Messages table.
- [ ] Frontend: Inbox UI (Left list, Right chat window).

**Week 10:**
- [ ] Feature: Review System (Stars + Comment).
- [ ] Logic: Only allow review if Order Status == Completed.
- [ ] Logic: Calculate Seller's average rating automatically.

#### Sprint 6: Differentiators & Polish
**Goal:** Implement one standout feature and finalize production hardening.

**Week 11 (Optional Features):**
- Option A (AI): Add an "AI Brief Generator" button on the search bar (Connects to OpenAI API).
- Option B (Video): Add "Video Call" button to Chat (using WebRTC/PeerJS).

**Week 12:**
- [ ] Bug Fixes: Handle edge cases (404 errors, Empty states).
- [ ] Docs: Generate Architecture Diagrams, ER Diagrams, and Screenshots.
- [ ] Deploy: Publish to chosen cloud (AWS or Azure — TBD).
